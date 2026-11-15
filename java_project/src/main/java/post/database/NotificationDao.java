package post.database;

import post.domain.Notification;

import java.util.List;

public interface NotificationDao
{
  List<Notification> getNotifications();
  Notification makeNotification(Notification notification);
  void readNotification(int id);
}
