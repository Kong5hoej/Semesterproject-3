package post.domain;

import jakarta.persistence.*;

import java.sql.Time;
import java.sql.Timestamp;
import java.time.Instant;
import java.util.HashSet;
import java.util.Objects;
import java.util.Set;

@Entity
@Table(name = "posts")
public class Post{
  @Id @GeneratedValue(strategy = GenerationType.AUTO)
  private Integer postID;
  private String title;
  private String description;
  private double price;
  private String zipCode;
  private boolean wantsToTrade;
  private String soldTo;
  private String timestamp;

  @Enumerated(EnumType.STRING)
  @Column(nullable = false)
  private PostState state;

  @ManyToOne(fetch = FetchType.LAZY)
  @JoinColumn(name="owner")
  private User owner;

  @ManyToMany(fetch = FetchType.EAGER)
  @JoinTable(
      name = "post_category",
      joinColumns = @JoinColumn(name = "post_id"),
      inverseJoinColumns = @JoinColumn(name = "category_name")
  )
  private Set<Category> categories = new HashSet<>();

  @OneToMany(mappedBy = "post", fetch = FetchType.EAGER)
  private Set<Image> images;

  @ManyToMany(fetch = FetchType.EAGER)
  private Set<User> favoritedBy;

  public Post(){}

  public Post(Integer PostID, String title, String description, double price,
              String zipCode, PostState state, boolean wantsToTrade, String soldTo, User owner, String timestamp){
    this.postID = PostID;
    this.title = title;
    this.description = description;
    this.price = price;
    this.zipCode = zipCode;
    this.state = state;
    this.wantsToTrade = wantsToTrade;
    this.soldTo = soldTo;
    this.owner = owner;
    this.categories = new HashSet<>();
    this.favoritedBy = new HashSet<>();
    this.timestamp = Timestamp.from(Instant.now()).toString();
  }

  public Post(String title, String description, double price, String zipCode, boolean wantsToTrade, User owner){
    this(null, title, description, price, zipCode, PostState.INACTIVE, wantsToTrade, "", owner, "");
  }

  // Getters and setters
  public int getId(){return postID;}
  public void setId(int postID){this.postID = postID;}

  public String getTitle(){return title;}
  public void setTitle(String title){this.title = title;}

  public String getDescription(){return description;}
  public void setDescription(String description){this.description = description;}

  public double getPrice(){return price;}
  public void setPrice(double price){
    this.price = price;
  }

  public String getZipCode(){
    return zipCode;
  }
  public void setZipCode(String zipCode){
    this.zipCode = zipCode;
  }

  public PostState getState(){return state;}
  public void setState(PostState state){this.state = state;}

  public boolean wantsToTrade(){return wantsToTrade;}
  public void setWantsToTrade(boolean wantsToTrade){this.wantsToTrade = wantsToTrade;}

  public String getSoldTo(){
    return soldTo;
  }
  public void setSoldTo(String soldTo){
    this.soldTo = soldTo;
  }

  public void setOwner(User owner){this.owner = owner;}
  public User getOwner(){return owner;}

  public void addCategory(Category category) {
    this.categories.add(category);
    category.getPosts().add(this);
  }
  public Set<Category> getCategories(){return categories;}
  public void clearCategories() {categories.clear();}

  public Set<Image> getImages(){
    if(images == null)
      return new HashSet<>();
    return images;
  }

  public String getTimestamp() { return timestamp; }
  public void setTimestamp(String timestamp) { this.timestamp = timestamp; }

  public Set<User> getFavoritedBy(){return favoritedBy;}

  @Override public boolean equals(Object obj){
    if(this == obj) return true;
    if(obj == null || getClass() != obj.getClass()) return false;

    Post other = (Post)obj;
    return Objects.equals(postID, other.postID);
  }

  @Override public int hashCode(){
    return Objects.hashCode(postID);
  }

  @Override public String toString(){
    return "";
        /*"Post{CategoryID=" + CategoryID + ", title='" + title + '\'' +
               ", description='" + description + '\'' + ", is_closed=" + is_closed + ", is_active=" + is_active + '}';*/
  }
}