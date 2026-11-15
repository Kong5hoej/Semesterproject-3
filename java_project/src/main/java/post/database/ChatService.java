package post.database;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import post.domain.Chat;
import post.domain.Message;
import post.domain.Post;
import post.domain.User;

import java.util.List;

@Service
public class ChatService implements ChatDao {
    @Autowired public ChatRepository chatRepository;

    @Override
    public Chat createChat(Post post, User user)
    {
        Chat chat = new Chat();

        chat.setPost(post);
        chat.setReceiver(user);
        chatRepository.save(chat);

        return chat;
    }

    public Chat getChatByUsername(String receiverUsername, int postId)
    {
        return chatRepository.findAll().stream()
            .filter(c -> c.getReceiver().getUsername().equals(receiverUsername) && c.getPost().getId() == postId)
            .findFirst()
            .orElseThrow(() -> new RuntimeException("Chatten med denne bruger findes ikke."));
    }

    @Override public List<Chat> getAllChats()
    {
        return chatRepository.findAll();
    }

    @Override
    public Chat getChatById(int chatId)
    {
        return chatRepository.findById(chatId).orElseThrow();
    }
}
