package post.database;

import post.domain.Chat;
import post.domain.Message;
import post.domain.Post;
import post.domain.User;

import java.util.List;

public interface ChatDao {
    Chat createChat(Post post, User user);
    List<Chat> getAllChats();
    Chat getChatById(int chatId);
}
