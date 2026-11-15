package post.server;

import io.grpc.stub.StreamObserver;
import post.database.NotificationDao;
import post.database.UserDao;
import post.domain.Notification;
import post.domain.NotificationType;
import post.domain.User;
import tradingForumGrpc.*;

import java.util.List;

public class NotificationServiceImpl extends NotificationServiceGrpc.NotificationServiceImplBase
{
  private final NotificationDao notificationDAO;
  private final UserDao userDAO;

  public NotificationServiceImpl(NotificationDao notificationDAO, UserDao userDAO)
  {
    this.notificationDAO = notificationDAO;
    this.userDAO = userDAO;
  }

  @Override public void getNotifications(Empty request,
      StreamObserver<GetNotificationsResponse> responseObserver)
  {
    List<Notification> notifications = notificationDAO.getNotifications();

    List<NotificationDTO> notificationDTOs = notifications.stream()
        .map(this::notificationDTOFromNotification)
        .toList();

    responseObserver.onNext(GetNotificationsResponse.newBuilder().addAllNotifications(notificationDTOs).build());
    responseObserver.onCompleted();
  }

  @Override public void makeNotification(CreateNotificationRequest request, StreamObserver<NotificationDTO> responseObserver)
  {
    Notification notification = new Notification(userDAO.getUserById(request.getUser()), NotificationType.getState(request.getNotificationType()), request.getObjectId().toString(), request.getMessage());
    Notification sendNotification = notificationDAO.makeNotification(notification);

    NotificationDTO response = notificationDTOFromNotification(sendNotification);

    responseObserver.onNext(response);
    responseObserver.onCompleted();
  }

  @Override public void readNotifications(ReadNotificationsRequest request,
      StreamObserver<Empty> responseObserver)
  {
    notificationDAO.readNotification(request.getId());

    responseObserver.onNext(Empty.newBuilder().build());
    responseObserver.onCompleted();
  }

  private NotificationDTO notificationDTOFromNotification(Notification notification)
  {
    NotificationDTO notificationDTO = NotificationDTO.newBuilder()
        .setUser(notification.getUser().getUsername())
        .setObjectId(notification.getObjectId())
        .setNotificationType(notification.getNotificationType().getValue())
        .setTimeStamp(notification.getCreatedAt())
        .setMessage(notification.getMessage())
        .setId(notification.getId())
        .build();
    return notificationDTO;
  }
}
