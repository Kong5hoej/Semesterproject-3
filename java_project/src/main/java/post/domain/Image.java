package post.domain;

import jakarta.persistence.*;

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;

@Entity
@Table(name = "images")
public class Image{
  @Id @GeneratedValue(strategy = GenerationType.AUTO)
  private int id;
  private String path;

  @ManyToOne(fetch = FetchType.EAGER)
  @JoinColumn(name="post")
  private Post post;

  public int getId(){return id;}

  public String getPath(){return path;}
  public void setPath(String path){this.path = path;}

  public Post getPost(){return post;}
  public void setPost(Post post){this.post = post;}

  public byte[] getImageData(){
    try{
      String filepath = "images/" + path;
      File file = new File(filepath);
      return Files.readAllBytes(file.toPath());
    } catch(IOException e){
      throw new RuntimeException(e);
    }
  }
}
