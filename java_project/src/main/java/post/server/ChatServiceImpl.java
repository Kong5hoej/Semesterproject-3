package post.server;

import io.grpc.stub.StreamObserver;
import post.database.ChatDao;
import post.database.PostDao;
import post.database.UserDao;
import post.domain.Chat;
import post.domain.Message;
import post.domain.Post;
import post.domain.User;
import tradingForumGrpc.*;

import java.util.ArrayList;
import java.util.List;

public class ChatServiceImpl extends ChatServiceGrpc.ChatServiceImplBase
{
  private final ChatDao chatDao;
  private final PostDao postDao;
  private final UserDao userDao;

  public ChatServiceImpl(ChatDao chatDao, PostDao postDao, UserDao userDao){
    this.chatDao = chatDao;
    this.postDao = postDao;
    this.userDao = userDao;
  }

  @Override public void getAllChats(Empty request, StreamObserver<GetAllChatsResponse> responseObserver)
  {
    List<Chat> chats = chatDao.getAllChats();
    List<ChatDTO> chatDTOs = chats.stream().map(this::chatDTOFromChat).toList();

    GetAllChatsResponse response = GetAllChatsResponse.newBuilder().addAllChats(chatDTOs).build();
    responseObserver.onNext(response);
    responseObserver.onCompleted();
  }

  @Override public void createChat(CreateChatDTO request, StreamObserver<GetChatResponse> responseObserver)
  {
    Post post = postDao.getPostById(request.getPostId());
    User user = userDao.getUserById(request.getUsername());

    ChatDTO chatDto = chatDTOFromChat(chatDao.createChat(post, user));

    GetChatResponse response = GetChatResponse.newBuilder().setChat(chatDto).build();
    responseObserver.onNext(response);
    responseObserver.onCompleted();
  }

  private MessageDTO messageDTOFromMessage(Message message)
  {
    MessageDTO messageDTO = MessageDTO.newBuilder()
        .setMessage(message.getMessage())
        .setSender(message.getSender().getUsername())
        .setTimeStamp(message.getTimestamp())
        .build();

    return messageDTO;
  }

  private ChatDTO chatDTOFromChat(Chat chat)
  {
      List<MessageDTO> messageDTOs = new ArrayList<>();

      for (Message message : chat.getMessages())
      {
          messageDTOs.add(messageDTOFromMessage(message));
      }

      ChatDTO chatDTO = ChatDTO.newBuilder()
          .setReceiver(chat.getReceiver().getUsername())
          .setPostId(chat.getPost().getId())
          .setChatId(chat.getChatID() == null ? -1 : chat.getChatID())
          .addAllMessages(messageDTOs).build();

      return chatDTO;
  }
}


