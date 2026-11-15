package post;

import io.grpc.ServerBuilder;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.ApplicationListener;
import org.springframework.context.event.ContextRefreshedEvent;
import org.springframework.stereotype.Component;
import post.database.*;
import post.server.*;

import java.io.IOException;

@SpringBootApplication
public class Server
{
  public static void main(String[] args) throws Exception
  {
    SpringApplication.run(Server.class, args);
  }

  @Component
  public class StartupApplication implements ApplicationListener<ContextRefreshedEvent>{
    @Autowired private PostDao postDao;
    @Autowired private UserDao userDao;
    @Autowired private CategoryDao categoryDao;
    @Autowired private ImageDao imageDao;
    @Autowired private ChatDao chatDao;
    @Autowired private MessageDao messageDao;
    @Autowired private NotificationDao notificationDao;

    // Gets called after Spring starts up
    @Override public void onApplicationEvent(ContextRefreshedEvent event){
      if(postDao.getAllPosts().isEmpty())
        new DummyData(categoryDao, chatDao, imageDao, messageDao, postDao, userDao, notificationDao);

      try{
        ServerBuilder.forPort(9090)
            .addService(new PostServiceImpl(postDao, userDao, imageDao, categoryDao))
            .addService(new UserServiceImpl(userDao, categoryDao))
            .addService(new CategoryServiceImpl(categoryDao))
            .addService(new ImageServiceImpl(imageDao))
            .addService(new ChatServiceImpl(chatDao, postDao, userDao))
            .addService(new MessageServiceImpl(messageDao, userDao, chatDao))
            .addService(new NotificationServiceImpl(notificationDao, userDao))
            .build().start();
      } catch(IOException e){
        System.out.println("gRPC endpoint failed to run");
      }
    }
  }
}
