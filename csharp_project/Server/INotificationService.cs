using ApiContracts;

namespace Domain;

public interface INotificationService
{
    public List<NotificationDTO> GetNotifications(string username);
    public NotificationDTO MakeNotification(CreateNotificationDTO createNotificationDTO);
    public void ReadNotifications(ReadNotificationsDTO readNotificationsDTO);
}