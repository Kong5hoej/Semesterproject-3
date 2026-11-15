namespace ApiContracts;

public class ChatDTO
{
    public int PostId { get; set; }
    public List<MessageDTO> Messages { get; set; }
    public string Receiver { get; set; }
    public string Sender { get; set; }
    public int Id { get; set; }
}