package post.database;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import post.domain.Notification;

import java.util.List;

@Service
public class NotificationService implements NotificationDao
{
  @Autowired public NotificationRepository repository;

  public List<Notification> getNotifications()
  {
    return repository.findAll();
  }

  public Notification makeNotification(Notification notification)
  {
    return repository.save(notification);
  }

  public void readNotification(int id)
  {
    repository.deleteById(id);
  }
}
