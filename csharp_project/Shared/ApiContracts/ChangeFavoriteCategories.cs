namespace ApiContracts;

public class ChangeFavoriteCategories
{
    public required string Username { get; set; }
    public required List<string> Categories { get; set; }
}