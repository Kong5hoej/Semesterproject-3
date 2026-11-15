package post.database;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import post.domain.Post;

@Repository
public interface PostRepository extends JpaRepository<Post, Integer>{ }
