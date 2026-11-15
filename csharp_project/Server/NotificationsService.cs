using ApiContracts;
using Grpc.Net.Client;
using TradingForumGrpc;
using NotificationDTO = ApiContracts.NotificationDTO;

namespace Domain;

public class NotificationsService : INotificationService
{
    private NotificationService.NotificationServiceClient Client { get; set; }
    
    public NotificationsService(NotificationService.NotificationServiceClient client)
    {
        Client = client;
    }

    public List<NotificationDTO> GetNotifications(String username)
    {
        TradingForumGrpc.GetNotificationsResponse
            response = Client.getNotifications(new Empty()); //Sender afsted til javaserver

        List<NotificationDTO> notificationDTOs = response.Notifications
            .Where(u => u.User.Equals(username))
            .Select(notificationDtoFromGrpc)
            .ToList();
        return notificationDTOs;
    }

    public ApiContracts.NotificationDTO MakeNotification(CreateNotificationDTO createNotificationDTO)
    {
        TradingForumGrpc.GetNotificationsResponse response = Client.getNotifications(new Empty()); //Sender afsted til javaserver
        if (response.Notifications
                .Where(u => u.User.Equals(createNotificationDTO.User))
                .Where(n => n.NotificationType.Equals(createNotificationDTO.NotificationType))
                .Where(o => o.ObjectId.Equals(createNotificationDTO.ObjectId))
                .Where(m => m.Message.Equals(createNotificationDTO.Message))
                .ToList().Capacity != 0)
        {
            return response.Notifications
                .Where(u => u.User.Equals(createNotificationDTO.User))
                .Where(n => n.NotificationType.Equals(createNotificationDTO.NotificationType))
                .Where(o => o.ObjectId.Equals(createNotificationDTO.ObjectId))
                .Where(m => m.Message.Equals(createNotificationDTO.Message))
                .Select(notificationDtoFromGrpc).First();
        }
        else
        {
            CreateNotificationRequest request = new CreateNotificationRequest{User = createNotificationDTO.User, NotificationType = createNotificationDTO.NotificationType, ObjectId = createNotificationDTO.ObjectId, Message = createNotificationDTO.Message};
            TradingForumGrpc.NotificationDTO notificationDto = Client.makeNotification(request); //Sender afsted til javaserver
            return new NotificationDTO(){User = notificationDto.User, NotificationType = notificationDto.NotificationType, ObjectId = notificationDto.ObjectId, Message = notificationDto.Message, Timestamp = notificationDto.TimeStamp};
        }
    }

    public void ReadNotifications(ReadNotificationsDTO readNotificationsDTO)
    {
        TradingForumGrpc.GetNotificationsResponse response = Client.getNotifications(new Empty()); //Sender afsted til javaserver
        
        TradingForumGrpc.NotificationDTO notification = response.Notifications
            .Where(u => u.User.Equals(readNotificationsDTO.User))
            .Where(n => n.NotificationType.Equals(readNotificationsDTO.NotificationType))
            .Where(o => o.ObjectId.Equals(readNotificationsDTO.ObjectId))
            .ToList().FirstOrDefault();

        if (notification == null)
        {
            return;
        }
        
        ReadNotificationsRequest request = new ReadNotificationsRequest{Id = notification.Id};
        Client.readNotifications(request); //Sender afsted til javaserver
    }
    
    private NotificationDTO notificationDtoFromGrpc(TradingForumGrpc.NotificationDTO notificationDto){
        return new NotificationDTO{
            User = notificationDto.User, 
            NotificationType = notificationDto.NotificationType, 
            ObjectId = notificationDto.ObjectId, 
            Timestamp = notificationDto.TimeStamp,
            Message = notificationDto.Message
        };
    }
}