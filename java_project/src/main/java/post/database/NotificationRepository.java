package post.database;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import post.domain.Notification;

@Repository
public interface NotificationRepository extends JpaRepository<Notification, Integer> { }