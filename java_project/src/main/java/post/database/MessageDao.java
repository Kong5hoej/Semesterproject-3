package post.database;

import post.domain.Message;

public interface MessageDao {
    void sendMessage(Message message);
}
