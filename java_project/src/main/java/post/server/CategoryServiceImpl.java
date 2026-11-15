package post.server;

import io.grpc.stub.StreamObserver;
import post.database.CategoryDao;
import post.database.UserDao;
import post.domain.Category;
import tradingForumGrpc.*;

import java.util.List;

public class CategoryServiceImpl extends CategoryServiceGrpc.CategoryServiceImplBase{
  private final CategoryDao categoryDao;

  public CategoryServiceImpl(CategoryDao categoryDao){
    this.categoryDao = categoryDao;
  }

  @Override public void getAllCategories(Empty request, StreamObserver<AllCategoriesResponse> responseObserver){
    List<Category> categories = categoryDao.getAllCategories();
    List<CategoryDTO> categoryDTOS = categories.stream()
        .map(category -> CategoryDTO.newBuilder().setName(category.getName()).build())
        .toList();

    AllCategoriesResponse response = AllCategoriesResponse.newBuilder().addAllCategories(categoryDTOS).build();
    responseObserver.onNext(response);
    responseObserver.onCompleted();
  }
}
