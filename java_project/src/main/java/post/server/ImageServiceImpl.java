package post.server;

import com.google.protobuf.ByteString;
import post.database.ImageDao;
import post.domain.Image;
import tradingForumGrpc.Empty;
import tradingForumGrpc.ImageDTO;
import tradingForumGrpc.ImageId;
import tradingForumGrpc.ImageServiceGrpc;

public class ImageServiceImpl extends ImageServiceGrpc.ImageServiceImplBase{
  private final ImageDao imageDao;

  public ImageServiceImpl(ImageDao imageDao){
    this.imageDao = imageDao;
  }

  @Override
  public void getImage(tradingForumGrpc.ImageId request,
      io.grpc.stub.StreamObserver<tradingForumGrpc.ImageDTO> responseObserver) {
    Image image = imageDao.getImageById(request.getId());

    ImageDTO imageDTO = ImageDTO.newBuilder()
        .setId(image.getId())
        .setData(ByteString.copyFrom(image.getImageData()))
        .build();

    responseObserver.onNext(imageDTO);
    responseObserver.onCompleted();
  }

  @Override
  public void addImage(tradingForumGrpc.AddImageRequest request,
      io.grpc.stub.StreamObserver<tradingForumGrpc.ImageId> responseObserver) {
    Image image = imageDao.createImage(request.getPostId(), request.getData().toByteArray());

    responseObserver.onNext(ImageId.newBuilder().setId(image.getId()).build());
    responseObserver.onCompleted();
  }

  @Override
  public void deleteImage(tradingForumGrpc.ImageId request,
      io.grpc.stub.StreamObserver<tradingForumGrpc.Empty> responseObserver) {
    imageDao.deleteImage(request.getId());

    responseObserver.onNext(Empty.newBuilder().build());
    responseObserver.onCompleted();
  }

}
