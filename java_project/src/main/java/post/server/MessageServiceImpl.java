package post.server;

import io.grpc.stub.StreamObserver;
import post.database.*;
import post.domain.*;
import tradingForumGrpc.*;

public class MessageServiceImpl extends MessageServiceGrpc.MessageServiceImplBase
{
  private final MessageDao messageDao;
  private final UserDao userDao;
  private final ChatDao chatDao;

  public MessageServiceImpl(MessageDao messageDao, UserDao userDao, ChatDao chatDao){
    this.messageDao = messageDao;
    this.userDao = userDao;
    this.chatDao = chatDao;
  }

  @Override public void sendMessage(SendMessageRequest request, StreamObserver<ChatDTO> responseObserver)
  {
    User user = userDao.getUserById(request.getMessage().getSender().toString());
    Chat chat = chatDao.getChatById(request.getMessage().getChatId());

    Message message = new Message(
        request.getMessage().getMessage(),
        user,
        chat
    );

    messageDao.sendMessage(message);

    responseObserver.onNext(
        ChatDTO.newBuilder()
            .setChatId(chat.getChatID())
            .setPostId(chat.getPost().getId())
            .setReceiver(message.getSender().getUsername())
            .addMessages(
                MessageDTO.newBuilder()
                    .setMessage(message.getMessage())
                    .setSender(message.getSender().getUsername())
            )
            .build()
    );
    responseObserver.onCompleted();
  }
}
