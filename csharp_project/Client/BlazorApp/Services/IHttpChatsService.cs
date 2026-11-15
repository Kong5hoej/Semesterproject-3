using ApiContracts;

namespace BlazorApp.Services;

public interface IHttpChatsService
{
    public Task<ChatDTO> MakeChatAsync(CreateChatDTO request);

    Task<List<ChatDTO>> GetChatsAsync(int postId);
    Task<ChatDTO> GetChatAsync(int postId, string buyer);
    Task<ChatDTO> GetChatAsync(int chatId);
    
    Task<List<ChatDTO>> GetChatsAsync(string buyer);
}