package post.database;

import org.springframework.data.jpa.repository.JpaRepository;
import post.domain.Chat;
import post.domain.Message;

public interface ChatRepository extends JpaRepository<Chat, Integer> {}
