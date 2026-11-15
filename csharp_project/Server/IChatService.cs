using ChatDTO = ApiContracts.ChatDTO;

namespace Domain;

public interface IChatsService
{
    public Task<ChatDTO> MakeChat(string username, int postId);
    public Task<List<ChatDTO>> GetAllChatsAsync(int? postId, string? username);
    public Task<ChatDTO> GetChat(string username, int postId);
    public ChatDTO GetChat(int chatId);
}