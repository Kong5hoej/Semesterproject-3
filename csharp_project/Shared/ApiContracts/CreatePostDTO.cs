namespace ApiContracts;

public class CreatePostDTO{
    public string? Title{ get; set; }
    public string? Description{ get; set; }
    public double Price{ get; set; }
    public string? ZipCode{ get; set; }
    public bool WantsToTrade{ get; set; }
    public required string Creator { get; set; }
    public List<string> Images { get; set; } = [];
    public List<CategoryDTO> Categories { get; set; } = [];
}