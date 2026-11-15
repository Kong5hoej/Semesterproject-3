package post.database;

import org.springframework.data.jpa.repository.JpaRepository;
import post.domain.Category;

public interface CategoryRepository extends JpaRepository<Category, String>{ }
