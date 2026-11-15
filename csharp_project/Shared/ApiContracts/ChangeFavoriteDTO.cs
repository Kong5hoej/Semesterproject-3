namespace ApiContracts;

public class ChangeFavoriteDTO{
    public required string User{ get; set; }
    public int PostId{ get; set; }
    public bool Favorite{ get; set; }
}