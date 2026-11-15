using ApiContracts;

namespace Domain;

public interface IMessagesService
{
    public Task SendMessage(SendMessageDTO sendMessageDto);
}