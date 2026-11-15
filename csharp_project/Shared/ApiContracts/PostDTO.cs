namespace ApiContracts;

public class PostDTO{
    public int Id{ get; set; }
    public string? Title{ get; set; }
    public string? Description{ get; set; }
    public double Price{ get; set; }
    public string? ZipCode{ get; set; }
    public bool WantsToTrade{ get; set; }
    public string? SoldTo{ get; set; }
    public int State { get; set; }
    public required string Owner{ get; set; }
    public required List<CategoryDTO> Categories{ get; set; }
    public List<int> ImageIds{ get; set; } = [];
    public required string Timestamp { get; set; }
}