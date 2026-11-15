package post.server;

import io.grpc.stub.StreamObserver;
import jakarta.transaction.Transactional;
import post.database.*;
import post.domain.*;
import tradingForumGrpc.*;

import java.util.ArrayList;
import java.util.List;

public class PostServiceImpl extends PostServiceGrpc.PostServiceImplBase
{
  private final PostDao postDAO;
  private final UserDao userDAO;
  private final ImageDao imageDAO;
  private final CategoryDao categoryDao;

  public PostServiceImpl(PostDao postDAO, UserDao userDao, ImageDao imageDao, CategoryDao categoryDao)
  {
    this.postDAO = postDAO;
    this.userDAO = userDao;
    this.imageDAO = imageDao;
    this.categoryDao = categoryDao;
  }

  @Override public void getPost(GetPostRequest request,
      StreamObserver<GetPostResponse> responseObserver)
  {
    Post post = postDAO.getPostById(request.getId());
    PostDTO postDTO = postDTOFromPost(post);

    responseObserver.onNext(
        GetPostResponse.newBuilder().setPost(postDTO).build());
    responseObserver.onCompleted();
  }

  @Transactional
  @Override public void createPost(CreatePostDTORequest request,
      StreamObserver<CreatePostDTOResponse> responseObserver)
  {
    Post post = new Post(
        request.getTitle(),
        request.getDescription(),
        request.getPrice(),
        request.getZipCode(),
        request.getWantsToTrade(),
        userDAO.getUserById(request.getCreator())
    );

    for(CategoryDTO category : request.getCategoriesList())
    {
      post.addCategory(categoryDao.getCategoryById(category.getName()));
    }
    post = postDAO.createPost(post);

    CreatePostDTOResponse response = CreatePostDTOResponse.newBuilder().setPostDto(postDTOFromPost(post)).build();

    responseObserver.onNext(response);
    responseObserver.onCompleted();
  }

  @Transactional
  @Override public void editPost(EditPostRequest request,
      StreamObserver<PostDTO> responseObserver) {
    Post post = postFromDTO(request.getPost());
    postDAO.editPost(request.getId(), post);

    List<Integer> currentIds = imageDAO.getImagesOnPost(request.getId()).stream().map(i -> i.getId()).toList();
    List<Integer> newIds = request.getPost().getImageIdsList().stream().map(i -> i.getId()).toList();
    List<Integer> deletedIds = new ArrayList<>();
    for(int imageId : currentIds){
      if(!newIds.contains(imageId)){
        deletedIds.add(imageId);
      }
    }
    for(int imageId : deletedIds){
      imageDAO.deleteImage(imageId);
    }

    responseObserver.onNext(postDTOFromPost(post));
    responseObserver.onCompleted();
  }

  @Override public void getAllPosts(Empty request,
      StreamObserver<GetPostsResponse> responseObserver) {
    List<Post> posts = postDAO.getAllPosts();

    List<PostDTO> postDTOs = posts.stream()
        .map(this::postDTOFromPost)
        .toList();

    GetPostsResponse response = GetPostsResponse.newBuilder()
        .addAllPost(postDTOs)
        .build();

    responseObserver.onNext(response);
    responseObserver.onCompleted();
  }

  private Post postFromDTO(PostDTO postDTO){
    Post post = new Post(
        postDTO.getId(),
        postDTO.getTitle(),
        postDTO.getDescription(),
        postDTO.getPrice(),
        postDTO.getZipCode(),
        PostState.getState(postDTO.getState()),
        postDTO.getWantsToTrade(),
        postDTO.getSoldTo(),
        userDAO.getUserById(postDTO.getOwner()),
        postDTO.getTimestamp());

    for(int i = 0; i < postDTO.getCategoriesCount(); i++)
    {
      post.addCategory(categoryDao.getCategoryById(postDTO.getCategories(i).getName()));
    }

    return post;
  }

  private PostDTO postDTOFromPost(Post post)
  {
    PostDTO postDTO = PostDTO.newBuilder()
        .setId(post.getId())
        .setTitle(post.getTitle())
        .setDescription(post.getDescription())
        .setPrice(post.getPrice())
        .setZipCode(post.getZipCode())
        .setState(post.getState().getValue())
        .setWantsToTrade(post.wantsToTrade())
        .setSoldTo(post.getSoldTo())
        .setOwner(post.getOwner().getUsername())
        .setTimestamp(post.getTimestamp())
        .addAllCategories(post.getCategories().stream()
            .map(category -> CategoryDTO.newBuilder().setName(category.getName())
                .build()).toList())
        .addAllImageIds(post.getImages().stream()
            .map(image -> ImageId.newBuilder().setId(image.getId())
                .build()).toList())
        .build();

    return postDTO;
  }
}
