package post.database;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import post.domain.User;

@Repository
public interface UserRepository extends JpaRepository<User, String>{ }
