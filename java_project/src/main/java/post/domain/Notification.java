package post.domain;

import jakarta.persistence.*;

import java.time.LocalDateTime;
import java.util.Arrays;

@Entity
@Table(name = "notifications")
public class Notification
{
  @Id
  @GeneratedValue(strategy = GenerationType.AUTO)
  private Integer id;

  @ManyToOne(fetch = FetchType.LAZY)
  @JoinColumn(name = "user_id", nullable = false)
  private User user;

  @Enumerated(EnumType.STRING)
  @Column(nullable = false)
  private NotificationType notificationType;

  private String objectId;

  private LocalDateTime createdAt;
  private String message;

  public Notification(){}
  public Notification(User user, NotificationType notificationType, String objectId, String message)
  {
    this.user = user;
    this.objectId = objectId;
    this.createdAt = LocalDateTime.now();
    this.notificationType = notificationType;
    this.message = message;
  }

  public Integer getId() {return id;}
  public void setUser(User user) {this.user = user;}
  public User getUser() {return user;}
  public void setNotificationType(NotificationType notificationType) {this.notificationType = notificationType;}
  public NotificationType getNotificationType() {return notificationType;}
  public void setObjectId(String objectId) {this.objectId = objectId;}
  public String getObjectId() {return objectId;}
  public String getCreatedAt() {return createdAt.toString();}
  public void setMessage(String message) { this.message = message;}
  public String getMessage() { return message;}
}
