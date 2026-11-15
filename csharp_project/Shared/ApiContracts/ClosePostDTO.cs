namespace ApiContracts;

public class ClosePostDTO{
    public int postId{ get; set; }
    public bool isSold{ get; set; }
    public string? soldTo{ get; set; }
}