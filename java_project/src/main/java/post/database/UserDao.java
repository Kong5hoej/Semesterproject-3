package post.database;

import post.domain.Category;
import post.domain.User;
import java.util.List;

public interface UserDao{
  User createUser(User user);
  String getHashedPassword(String username);
  void editUser(String username, User updatedUser);
  List<User> getAllUsers();
  void changeFavorite(String user, int postId, boolean favorite);
  void changeFavoriteUser(String username, String favoriteUsername, boolean favorite);
  User getUserById(String username);
  void save(User user);
  List<User> getAllFavoritedBy(String username);
  List<Category> getAllFavoritedCategories(String username);
  void changeFavoriteCategory(String username, String categoryName);
  void deleteAllFavoritedCategories(String username);
}
