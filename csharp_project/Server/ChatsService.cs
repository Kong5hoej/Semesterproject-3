using Grpc.Net.Client;
using TradingForumGrpc;
using ChatDTO = ApiContracts.ChatDTO;
using MessageDTO = ApiContracts.MessageDTO;

namespace Domain;

public class ChatsService : IChatsService {
    private ChatService.ChatServiceClient Client { get; set; }

    public ChatsService(ChatService.ChatServiceClient client)
    {
        Client = client;
    }

    public async Task<ChatDTO> MakeChat(string username, int postId)
    {
        if (postId == null || postId == 0)
            throw new Exception("Der Mangler Et ID På Din Post!");

        TradingForumGrpc.CreateChatDTO request = new TradingForumGrpc.CreateChatDTO{ PostId = postId, Username = username};
        GetChatResponse chat = await Client.createChatAsync(request); //Sender afsted til javaserver
        ChatDTO chatDto = chatDtoFromGrpc(chat.Chat);

        return chatDto;
    }

    public async Task<List<ChatDTO>> GetAllChatsAsync(int? postId, string? username)
    {
        GetAllChatsResponse response = await Client.getAllChatsAsync(new Empty());
        IEnumerable<TradingForumGrpc.ChatDTO> filteredChats = response.Chats;
        
        if (username != null){
            filteredChats= filteredChats
                .Where(c => c.Receiver == username);
        }
        
        if (postId != null){
            filteredChats = filteredChats
                .Where(c => c.PostId == postId);
        }
        
        return filteredChats
            .Select(chatDtoFromGrpc)
            .ToList();
    }

    public async Task<ChatDTO> GetChat(string username, int postId)
    {
        ChatDTO chat;
        TradingForumGrpc.GetAllChatsResponse response = Client.getAllChats(new Empty());
        try
        {
            chat = response.Chats
                .Where(p => p.PostId.Equals(postId))
                .Where(u => u.Receiver.Equals(username))
                .Select(chatDtoFromGrpc)
                .First();
        }
        catch (Exception e)
        {
            chat = null;
        }

        if (chat == null)
        {
            chat = await MakeChat(username, postId);
        }
        return chat;
    }

    public ChatDTO GetChat(int chatId)
    {
        TradingForumGrpc.GetAllChatsResponse response = Client.getAllChats(new Empty());
        return response.Chats
            .Where(c => c.ChatId.Equals(chatId))
            .Select(chatDtoFromGrpc)
            .First();
    }

    private MessageDTO messageDtoFromGrpc(TradingForumGrpc.MessageDTO messageDto)
    {
        return new MessageDTO(){
            Sender = messageDto.Sender, TimeStamp = messageDto.TimeStamp, Message = messageDto.Message
        };
    }

    private ChatDTO chatDtoFromGrpc(TradingForumGrpc.ChatDTO chatDto)
    {
        List<MessageDTO> messages = new List<MessageDTO>();
        for (int i = 0; i < chatDto.Messages.Count; i++){
            messages.Add(new MessageDTO(){
                Message = chatDto.Messages[i].Message, Sender = chatDto.Messages[i].Sender, TimeStamp = chatDto.Messages[i].TimeStamp
            });
        }
        return new ChatDTO(){
            Messages = messages, PostId = chatDto.PostId, Sender = chatDto.Receiver, Id = chatDto.ChatId
        };
    }
}