package post.database;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import post.domain.Category;

import java.util.List;

@Service
public class CategoryService implements CategoryDao{
  @Autowired
  public CategoryRepository repository;

  @Override
  public void createCategory(Category category)
  {
    repository.save(category);
  }

  @Override public List<Category> getAllCategories(){
    return repository.findAll();
  }

  @Override public Category getCategoryById(String name)
  {
    return repository.findById(name).orElseThrow();
  }

  @Override public void saveCategory(Category category)
  {
    repository.save(category);
  }
}
