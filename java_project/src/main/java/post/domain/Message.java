package post.domain;

import jakarta.persistence.*;

import java.sql.Timestamp;
import java.time.Instant;

@Entity
@Table(name = "messages")
public class Message {

    @Id
    @GeneratedValue(strategy = GenerationType.AUTO)
    private Integer id;

    private String timestamp;
    private String message;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="username")
    private User sender;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="chatID")
    private Chat chat;

    public Message(){}

    public Message(String message, User sender, Chat chat)
    {
        this.message = message;
        this.sender = sender;
        this.chat = chat;
        this.timestamp = Timestamp.from(Instant.now()).toString();
    }

    public Integer getId() {return id;}
    public String getTimestamp() {return timestamp;}
    public String getMessage() {return message;}
    public void setMessage(String message) {this.message = message;}
    public User getSender() {return sender;}
    public void setSender(User sender) {this.sender = sender;}
    public Chat getChat() {return chat;}
    public void setChat(Chat chat) {this.chat = chat;}




}
