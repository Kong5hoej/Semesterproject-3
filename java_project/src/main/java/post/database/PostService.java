package post.database;

import jakarta.transaction.Transactional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import post.domain.Category;
import post.domain.Post;

import java.util.ArrayList;
import java.util.List;

@Service
public class PostService implements PostDao
{
  @Autowired private PostRepository repository;
  @Autowired private CategoryDao cDao;

  @Transactional
  @Override public Post createPost(Post post)
  {
    Post savedPost = repository.save(post);
    for (int i = 0; i < post.getCategories().size(); i++)
    {
      List<Category> categoryList = new ArrayList<>(post.getCategories());
      categoryList.get(i).addPost(post);
      cDao.saveCategory(categoryList.get(i));
    }
    return savedPost;
  }

  @Override public Post getPostById(int postId){
    return repository.findById(postId).orElse(null);
  }

  @Transactional
  @Override public void editPost(int postId, Post updatedPost){
    Post oldPost = repository.findById(postId).orElseThrow();
    oldPost.setTitle(updatedPost.getTitle());
    oldPost.setDescription(updatedPost.getDescription());
    oldPost.setPrice(updatedPost.getPrice());
    oldPost.setZipCode(updatedPost.getZipCode());
    oldPost.setState(updatedPost.getState());
    oldPost.setWantsToTrade(updatedPost.wantsToTrade());
    oldPost.setSoldTo(updatedPost.getSoldTo());
    oldPost.clearCategories();

    for (int i = 0; i < updatedPost.getCategories().size(); i++)
    {
      List<Category> categoryList = new ArrayList<>(updatedPost.getCategories());
      categoryList.get(i).addPost(oldPost);
      cDao.saveCategory(categoryList.get(i));
    }
    repository.save(oldPost);
  }

  @Override public List<Post> getAllPosts(){
    return repository.findAll();
  }

  @Override public void savePost(Post post)
  {
    repository.save(post);
  }
}
