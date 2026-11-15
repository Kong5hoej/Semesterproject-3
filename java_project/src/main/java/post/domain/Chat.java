package post.domain;

import jakarta.persistence.*;

import java.sql.Timestamp;
import java.util.ArrayList;
import java.util.List;

@Entity
@Table(name = "chats")

public class Chat {
    @Id @GeneratedValue(strategy = GenerationType.AUTO)
    private Integer chatID;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="username")
    private User receiver;

    @ManyToOne(fetch = FetchType.EAGER)
    @JoinColumn(name="postID")
    private Post post;

    @OneToMany(mappedBy = "chat", fetch = FetchType.EAGER)
    private List<Message> messages = new ArrayList<>();

    public Chat(){}

    public void setMessages(List<Message> messages) {messages.addAll(messages);}
    public List<Message> getMessages() {return messages;}
    public User getReceiver() {return receiver;}
    public void setReceiver(User receiver) {this.receiver = receiver;}
    public Post getPost() {return post;}
    public void setPost(Post post) {this.post = post;}
    public Integer getChatID() {return chatID;}
}
