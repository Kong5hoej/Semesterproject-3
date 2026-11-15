package post.database;

import post.domain.Category;
import java.util.List;

public interface CategoryDao {
    void createCategory(Category category);
    List<Category> getAllCategories();
    Category getCategoryById(String name);
    void saveCategory(Category category);
}
