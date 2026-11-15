package post.database;

import post.domain.*;

import java.io.IOException;
import java.net.URL;


public class DummyData
{
  CategoryDao categoryDao;
  ChatDao chatDao;
  ImageDao imageDao;
  MessageDao messageDao;
  PostDao postDao;
  UserDao userDao;
  NotificationDao notificationDao;

  public DummyData(CategoryDao categoryDao, ChatDao chatDao, ImageDao imageDao, MessageDao messageDao, PostDao postDao, UserDao userDao, NotificationDao notificationDao)
  {
    this.categoryDao = categoryDao;
    this.chatDao = chatDao;
    this.imageDao = imageDao;
    this.messageDao = messageDao;
    this.postDao = postDao;
    this.userDao = userDao;
    this.notificationDao = notificationDao;

    createCategories();
    createUsers();
    createPostsAndALotOfOtherThings();
  }

  public void createCategories()
  {
    categoryDao.createCategory(new Category("Elektronik"));
    categoryDao.createCategory(new Category("Sofa"));
    categoryDao.createCategory(new Category("Båd"));
    categoryDao.createCategory(new Category("Bil"));
    categoryDao.createCategory(new Category("Flyvemaskiner"));
    categoryDao.createCategory(new Category("Helikopter"));
    categoryDao.createCategory(new Category("Legetøj"));
    categoryDao.createCategory(new Category("Seng"));
    categoryDao.createCategory(new Category("Spisebord"));
    categoryDao.createCategory(new Category("Stole"));
  }

  public void createUsers()
  {
    userDao.createUser(new User("bob", "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=", ""));
    userDao.createUser(new User("admin", "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=", ""));
  }

  public void createPostsAndALotOfOtherThings()
  {
    Post post1 = postDao.createPost(new Post("Sofa til salg!", "Hej, jeg sælger min fantastiske sofa, da jeg skal have en mindre!", 100, "8700", false, userDao.getUserById("bob")));
    post1.addCategory(categoryDao.getCategoryById("Sofa"));
    post1.setState(PostState.ACTIVE);
    postDao.editPost(post1.getId(), post1);

    Post post2 = postDao.createPost(new Post("Billig bil!", "Hej, jeg sælger min dejlige bil, da jeg skal have en større! Byttehandler modtages gerne!", 10000, "8700", true, userDao.getUserById("admin")));
    post2.addCategory(categoryDao.getCategoryById("Bil"));
    post2.setState(PostState.ACTIVE);
    postDao.editPost(post2.getId(), post2);

    Post post3 = postDao.createPost(new Post("Billig flyvemaskine!", "Hej, jeg sælger min dejlige flyver, da jeg skal have en større! Byttehandler modtages gerne!", 150000, "6000", true, userDao.getUserById("bob")));
    post3.addCategory(categoryDao.getCategoryById("Flyvemaskiner"));
    post3.setState(PostState.ACTIVE);
    postDao.editPost(post3.getId(), post3);

    Post post4 = postDao.createPost(new Post("Spisebord til salg!", "Hej, jeg sælger mit fantastiske spisebord, da jeg skal have et større!", 1200, "8700", false, userDao.getUserById("admin")));
    post4.addCategory(categoryDao.getCategoryById("Spisebord"));
    post4.setState(PostState.INACTIVE);
    postDao.editPost(post4.getId(), post4);

    Post post5 = postDao.createPost(new Post("Telefon til salg!", "Hej, jeg sælger min dejlige telefon, da jeg skal have en ny!", 1000, "2000", false, userDao.getUserById("bob")));
    post5.addCategory(categoryDao.getCategoryById("Elektronik"));
    post5.setState(PostState.INACTIVE);
    postDao.editPost(post5.getId(), post5);

    Post post6 = postDao.createPost(new Post("Computer til salg!", "Hej, jeg sælger min dejlige computer, da jeg skal have en ny!", 2500, "2000", false, userDao.getUserById("bob")));
    post6.addCategory(categoryDao.getCategoryById("Elektronik"));
    post6.setState(PostState.INACTIVE);
    postDao.editPost(post6.getId(), post6);

    Post post7 = postDao.createPost(new Post("Skærm til salg!", "Hej, jeg sælger min dejlige skærm, da jeg skal have en ny!", 1400, "8660", false, userDao.getUserById("admin")));
    post7.addCategory(categoryDao.getCategoryById("Elektronik"));
    post7.setState(PostState.INACTIVE);
    postDao.editPost(post7.getId(), post7);

    try
    {
      byte[] bytes = new URL(
          "https://i.ebayimg.com/images/g/bWsAAOSwvXBjFJ14/s-l1200.jpg").openStream()
          .readAllBytes();
      imageDao.createImage(post1.getId(), bytes);
    }
    catch (IOException e)
    {
      throw new RuntimeException("Billedet eksisterer ikke");
    }

    notificationDao.makeNotification(new Notification(userDao.getUserById("admin"), NotificationType.LISTING_FROM_USER, ""+post1.getId(),"Nyt/ændret opslag fra bob"));
    notificationDao.makeNotification(new Notification(userDao.getUserById("bob"), NotificationType.LISTING_FROM_USER, ""+post2.getId(),"Nyt/ændret opslag fra admin"));

    userDao.changeFavorite(userDao.getUserById("bob").getUsername(), post2.getId(), true);
    userDao.changeFavorite(userDao.getUserById("admin").getUsername(), post1.getId(), true);

    Chat chat = chatDao.createChat(postDao.getPostById(post1.getId()), userDao.getUserById("admin"));

    messageDao.sendMessage(new Message("Hej, jeg vil gerne byde 50 kroner for din sofa!", userDao.getUserById("admin"), chatDao.getChatById(chat.getChatID())));
    messageDao.sendMessage(new Message("Det er for lavt, hvad siger du til 75 kroner?", userDao.getUserById("bob"), chatDao.getChatById(chat.getChatID())));
    messageDao.sendMessage(new Message("Det er en aftale, jeg er på vej!", userDao.getUserById("admin"), chatDao.getChatById(chat.getChatID())));

    notificationDao.makeNotification(new Notification(userDao.getUserById("bob"), NotificationType.MESSAGE, chat.getChatID().toString(), "Ny besked fra admin"));
    notificationDao.makeNotification(new Notification(userDao.getUserById("admin"), NotificationType.MESSAGE, chat.getChatID().toString(), "Ny besked fra bob"));

    userDao.changeFavoriteCategory(userDao.getUserById("bob").getUsername(), categoryDao.getCategoryById("Elektronik").getName());
    userDao.changeFavoriteCategory(userDao.getUserById("bob").getUsername(), categoryDao.getCategoryById("Legetøj").getName());
    userDao.changeFavoriteCategory(userDao.getUserById("bob").getUsername(), categoryDao.getCategoryById("Seng").getName());
    userDao.changeFavoriteCategory(userDao.getUserById("bob").getUsername(), categoryDao.getCategoryById("Spisebord").getName());

    userDao.changeFavoriteCategory(userDao.getUserById("admin").getUsername(), categoryDao.getCategoryById("Elektronik").getName());
    userDao.changeFavoriteCategory(userDao.getUserById("admin").getUsername(), categoryDao.getCategoryById("Flyvemaskiner").getName());
    userDao.changeFavoriteCategory(userDao.getUserById("admin").getUsername(), categoryDao.getCategoryById("Sofa").getName());

    userDao.changeFavoriteUser(userDao.getUserById("bob").getUsername(), userDao.getUserById("admin").getUsername(), true);
    userDao.changeFavoriteUser(userDao.getUserById("admin").getUsername(), userDao.getUserById("bob").getUsername(), true);
  }
}
