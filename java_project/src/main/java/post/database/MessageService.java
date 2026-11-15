package post.database;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import post.domain.Message;

@Service
public class MessageService implements MessageDao {
    @Autowired public MessageRepository messageRepository;

    @Override
    public void sendMessage(Message message) {
        messageRepository.save(message);
    }
}

