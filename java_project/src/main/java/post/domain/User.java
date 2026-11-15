package post.domain;

import jakarta.persistence.*;

import java.util.HashSet;
import java.util.Objects;
import java.util.Set;

@Entity
@Table(name="users")
public class User
{
  @Id
  private String username;
  private String password;
  private String zipCode;

  @OneToMany(mappedBy = "owner", fetch = FetchType.LAZY)
  private Set<Post> posts;

  @ManyToMany(mappedBy = "favoritedBy", fetch = FetchType.EAGER)
  private Set<Post> favorites;

  @ManyToMany(mappedBy = "favoritedByUsers", fetch = FetchType.EAGER)
  private Set<User> favoriteUsers;

  @ManyToMany(mappedBy = "categoriesFavoritedByUsers", fetch = FetchType.EAGER)
  private Set<Category> favoriteCategories;

  @ManyToMany(fetch = FetchType.EAGER)
  private Set<User> favoritedByUsers;

  public User(){}

  public User(String username, String password, String zipCode)
  {
    this.username = username;
    this.password = password;
    this.zipCode = zipCode;
    this.posts = new HashSet<>();
    this.favorites = new HashSet<>();
    this.favoriteUsers = new HashSet<>();
    this.favoritedByUsers = new HashSet<>();
    this.favoriteCategories = new HashSet<>();
  }

  public void setUsername(String username){this.username = username;}
  public String getUsername() {return username;}

  public void setPassword(String password){this.password = password;}
  public String getPassword() {return password;}

  public void setZipCode(String zipCode){this.zipCode = zipCode;}
  public String getZipCode() {return zipCode;}

  public Set<Post> getFavorites(){return favorites;}

  public Set<User> getFavoriteUsers(){return favoriteUsers;}
  public Set<User> getFavoritedByUsers(){return favoritedByUsers;}
  public Set<Category> getFavoriteCategories(){return favoriteCategories;}

  @Override public boolean equals(Object obj){
    if(this == obj) return true;
    if(obj == null || getClass() != obj.getClass()) return false;

    User other = (User)obj;
    return Objects.equals(username, other.username);
  }

  @Override public int hashCode(){
    return username.hashCode();
  }
}
