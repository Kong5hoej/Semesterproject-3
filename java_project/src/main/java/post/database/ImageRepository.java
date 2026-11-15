package post.database;

import org.springframework.data.jpa.repository.JpaRepository;
import post.domain.Image;

public interface ImageRepository extends JpaRepository<Image, Integer>{ }
