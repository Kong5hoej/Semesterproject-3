package post.database;
import post.domain.Post;

import java.util.List;


public interface PostDao {
    Post createPost(Post post);
    void editPost(int postId, Post updatedPost);
    List<Post> getAllPosts();
    Post getPostById(int postId);
    void savePost(Post post);
}