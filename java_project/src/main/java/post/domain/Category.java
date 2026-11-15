package post.domain;

import jakarta.persistence.*;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

@Entity
@Table(name = "categories")
public class Category
{
  @Id
  private String name;

  @ManyToMany (mappedBy = "categories", fetch = FetchType.EAGER)
  private Set<Post> posts = new HashSet<>();

  @ManyToMany(fetch = FetchType.EAGER)
  private Set<User> categoriesFavoritedByUsers = new HashSet<>();

  public Category(){}

  public Category(String name)
  {
    this.name = name;
  }

  public String getName(){return name;}

  public void addPost(Post post) {
    this.posts.add(post);
    post.getCategories().add(this);
  }

  public void addUser(User user)
  {
    this.categoriesFavoritedByUsers.add(user);
    user.getFavoriteCategories().add(this);
  }

  public Set<User> getUsers()
  {
    return categoriesFavoritedByUsers;
  }

  public Set<Post> getPosts()
  {
    return posts;
  }
}
