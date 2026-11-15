using ApiContracts;

namespace BlazorApp.Services;

public interface IHttpMessagesService
{
    Task SendMessage(int postId, string buyer, MessageDTO request);

    Task<List<MessageDTO>> GetMessagesAsync(int postId, int chatId);
}