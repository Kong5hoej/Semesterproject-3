using ApiContracts;

namespace BlazorApp.Services;

public interface IHttpUsersService
{
    public Task<UserDTO> CreateUserAsync(CreateUserDTO request);
    public Task<UserDTO> getUserAsync(string username);
    public Task<List<int>> GetFavoritesAsync(string username);
    public Task ChangeFavoriteAsync(string username, int postId, bool favorite);
    public Task EditUserAsync(UserDTO request, string username);
    public Task<List<String>> GetFavoriteUsersAsync(string username);
    public Task ChangeFavoriteUserAsync(string username, string favoriteUser, bool favorite);
    public Task<List<CategoryDTO>> GetFavoriteCategoriesAsync(string username);
    public Task ChangeFavoriteCategoriesAsync(string username, List<string> categories);
}   