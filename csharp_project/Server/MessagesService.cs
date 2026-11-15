using ApiContracts;
using Grpc.Net.Client;
using TradingForumGrpc;
using MessageDTO = ApiContracts.MessageDTO;

namespace Domain;

public class MessagesService : IMessagesService
{
    private MessageService.MessageServiceClient Client { get; set; }
    private IChatsService ChatsService { get; set; }
    private INotificationService NotificationService { get; set; }
    private IPostsService PostService { get; set; }
    
    public MessagesService(MessageService.MessageServiceClient client, IChatsService chatsService, INotificationService notificationService, IPostsService postService)
    {
        Client = client;
        ChatsService = chatsService;
        NotificationService = notificationService;
        PostService = postService;
    }
    
    public async Task SendMessage(SendMessageDTO sendMessageDto)
    {
        if (sendMessageDto.Message == null || string.IsNullOrEmpty(sendMessageDto.Message.Message))
            throw new Exception("Din besked er tom!");
        
        ApiContracts.ChatDTO chat;
        try
        {
            chat = (await ChatsService.GetAllChatsAsync(sendMessageDto.PostId, null)) //Er det receiver eller sender?
                .First(u => u.Sender.Equals(sendMessageDto.Buyer));
        }
        catch (Exception e)
        {
            chat = null;
        }

        if (chat == null)
        {
            chat = await ChatsService.MakeChat(sendMessageDto.Buyer, sendMessageDto.PostId);
        }
        TradingForumGrpc.SendMessageRequest request = new TradingForumGrpc.SendMessageRequest(){Message = new TradingForumGrpc.MessageDTO() { Sender = sendMessageDto.Message.Sender, Message = sendMessageDto.Message.Message, ChatId = chat.Id}, PostId = sendMessageDto.PostId, Buyer = sendMessageDto.Buyer};
        TradingForumGrpc.ChatDTO sentMessage = Client.sendMessage(request); //Sender afsted til javaserver
        
        if (sentMessage.Receiver.Equals(sendMessageDto.Buyer))
        {
            ApiContracts.PostDTO post = PostService .GetPosts()
                .First(p => p.Id == sentMessage.PostId);
            NotificationService.MakeNotification(new CreateNotificationDTO()
            {
                User = post.Owner, 
                Message = newMessage(sentMessage.Receiver), 
                NotificationType = 0,
                ObjectId = sentMessage.ChatId.ToString()
            });
        }
        else
        {
            
            NotificationService.MakeNotification(new CreateNotificationDTO()
            {
                User = sendMessageDto.Buyer, 
                Message = newMessage(sentMessage.Receiver), 
                NotificationType = 0,
                ObjectId = sentMessage.ChatId.ToString()
            });
        }
    }

    private string newMessage(string sender)
    {
        return $"Ny besked fra {sender}";
    }
}