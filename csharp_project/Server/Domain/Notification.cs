namespace Domain;

public class Notification
{
    public string User { get; set; }
    public int NotificationType { get; set; }
    public string ObjectId { get; set; }
    public string Timestamp { get; set; }

    public Notification(string user,  int notificationType, string objectId)
    {
        this.User = user;
        this.NotificationType = notificationType;
        this.ObjectId = objectId;
        this.Timestamp = null;
    }

    public void SetUser(String user) { this.User = user; }
    public String GetUser() {return User;}
    
    public void SetNotificationType(int notificationType) { this.NotificationType = notificationType; }
    public int GetNotificationType() { return NotificationType; }
    
    public void SetObjectId(string objectId) { this.ObjectId = objectId; }
    public string  GetObjectId() { return ObjectId; }
    
    public string GetTimestamp() { return Timestamp; }

}