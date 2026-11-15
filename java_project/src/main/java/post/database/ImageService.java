package post.database;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import post.domain.Image;

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.util.List;
import java.util.Scanner;

@Service
public class ImageService implements ImageDao{
  @Autowired
  private ImageRepository repository;
  @Autowired
  private PostDao postDao;

  @Override public Image createImage(int postId, byte[] bytes){
    Image image = new Image();
    image.setPost(postDao.getPostById(postId));

    image = repository.save(image);
    image.setPath("" + image.getId());
    image = repository.save(image);

    File file = new File("images/" + image.getPath());
    try{
      if(!Files.exists(file.getParentFile().toPath()))
        Files.createDirectory(file.getParentFile().toPath());
      Files.write(file.toPath().toAbsolutePath(), bytes);
    } catch(IOException e){
      e.printStackTrace();
    }

    return image;
  }

  @Override public Image getImageById(int id){
    return repository.findById(id).orElseThrow();
  }

  @Override public List<Image> getImagesOnPost(int postId){
    return repository.findAll().stream().filter(image -> image.getPost().getId() == postId).toList();
  }

  @Override public void deleteImage(int id){
    repository.deleteById(id);
  }
}
