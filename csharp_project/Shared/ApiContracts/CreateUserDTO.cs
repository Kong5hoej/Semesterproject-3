namespace ApiContracts;

public class CreateUserDTO
{
    public string Username{ get; set; }
    public string HashPassword{ get; set; }
    public string? ZipCode{ get; set; }
}