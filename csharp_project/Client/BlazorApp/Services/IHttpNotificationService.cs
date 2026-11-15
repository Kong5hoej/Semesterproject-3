using ApiContracts;

namespace BlazorApp.Services;

public interface IHttpNotificationService
{
    public Task<List<NotificationDTO>> GetNotifications(string username);
    public Task ReadNotifications(ReadNotificationsDTO readNotificationsDTO);
}