package post.database;

import post.domain.Image;

import java.io.OutputStream;
import java.util.List;

public interface ImageDao{
  Image createImage(int postId, byte[] bytes);
  List<Image> getImagesOnPost(int postId);
  Image getImageById(int id);
  void deleteImage(int id);
}
