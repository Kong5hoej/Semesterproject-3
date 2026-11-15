namespace ApiContracts;

public class CreateNotificationDTO
{
    public string User { get; set; }
    public int NotificationType { get; set; }
    public string ObjectId { get; set; }
    public string Message { get; set; }
}