using ApiContracts;

namespace Domain;

public interface IPostsService
{
    public Task<PostDTO> MakePost(CreatePostDTO createPostDto);
    public Task ChangePostState(ChangePostStateDTO changePostStateDto);
    public Task ClosePost(ClosePostDTO closePostDto);
    public Task ChangePost(PostDTO postDto);
    public List<PostDTO> GetPosts();
    public PostDTO GetPost(int postId);
}