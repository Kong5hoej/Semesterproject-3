namespace ApiContracts;

public class SendMessageDTO
{
    public int PostId { get; set; }
    public string Buyer { get; set; }
    public MessageDTO Message { get; set; }
}