package post.database;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import post.domain.Category;
import post.domain.Post;
import post.domain.User;
import java.util.List;

@Service
public class UserService implements UserDao{
  @Autowired
  private UserRepository repository;
  @Autowired
  private PostDao postDao;
  @Autowired private CategoryDao categoryDao;

  @Override public User createUser(User user)
  {
    return repository.save(user);
  }

  @Override public User getUserById(String username){
    return repository.findById(username).orElseThrow();
  }

  @Override public String getHashedPassword(String username){
    return getUserById(username).getPassword();
  }

  @Override
  public void editUser(String username, User updatedUser) {
    User user = repository.findById(username).orElseThrow();
    user.setZipCode(updatedUser.getZipCode());
    repository.save(user);
  }

  @Override public List<User> getAllUsers(){
    return repository.findAll();
  }

  @Override
  public void changeFavorite(String username, int postId, boolean favorite){
    User user = getUserById(username);
    Post post = postDao.getPostById(postId);

    if(favorite){
      user.getFavorites().add(post);
      post.getFavoritedBy().add(user);
    } else {
      user.getFavorites().remove(post);
      post.getFavoritedBy().remove(user);
    }

    repository.save(user);
    postDao.savePost(post);
  }

  @Override
  public void changeFavoriteUser(String username, String favoriteUsername, boolean favorite) {
    User user = getUserById(username);
    User favoriteUser = getUserById(favoriteUsername);

    if (favorite){
      user.getFavoriteUsers().add(favoriteUser);
      favoriteUser.getFavoritedByUsers().add(user);
    }
    else {
      user.getFavoriteUsers().remove(favoriteUser);
      favoriteUser.getFavoritedByUsers().remove(user);
    }
    repository.save(user);
    repository.save(favoriteUser);
  }

  @Override
  public List<User> getAllFavoritedBy(String username) {
    User user = getUserById(username);
    if (user == null) {
      return List.of();
    }
    return user.getFavoritedByUsers().stream().toList();
  }

  @Override
  public void save(User user)
  {
    repository.save(user);
  }

  @Override
  public List<Category> getAllFavoritedCategories(String username)
  {
    User user = getUserById(username);
    if (user == null) {
      return List.of();
    }
    return user.getFavoriteCategories().stream().toList();
  }

  @Override
  public void changeFavoriteCategory(String username, String categoryName)
  {
    User user = getUserById(username);
    Category category = categoryDao.getCategoryById(categoryName);

    user.getFavoriteCategories().add(category);
    category.getUsers().add(user);

    repository.save(user);
    categoryDao.saveCategory(category);
  }

  @Override
  public void deleteAllFavoritedCategories(String username)
  {
    List<Category> categories = getAllFavoritedCategories(username);
    User user = getUserById(username);

    for(int i = 0; i < categories.size(); i++)
    {
      Category category = categoryDao.getCategoryById(categories.get(i).getName());

      user.getFavoriteCategories().remove(category);
      category.getUsers().remove(user);

      repository.save(user);
      categoryDao.saveCategory(category);
    }
  }
}
