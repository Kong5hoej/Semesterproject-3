namespace ApiContracts;

public class ChangeFavoriteUserDTO {
    public required string Username { get; set; }
    public required string FavoriteUsername { get; set; }
    public required bool Favorite { get; set; }
}