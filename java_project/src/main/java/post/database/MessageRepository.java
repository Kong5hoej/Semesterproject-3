package post.database;

import org.springframework.data.jpa.repository.JpaRepository;
import post.domain.Message;

public interface MessageRepository extends JpaRepository<Message, String>{}
