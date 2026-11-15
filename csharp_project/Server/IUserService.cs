using ApiContracts;
using TradingForumGrpc;
using CategoryDTO = ApiContracts.CategoryDTO;
using ChangeFavoriteDTO = ApiContracts.ChangeFavoriteDTO;
using ChangeFavoriteUserDTO = ApiContracts.ChangeFavoriteUserDTO;
using UserDTO = ApiContracts.UserDTO;

namespace Domain;

public interface IUserService {
    public Task<UserDTO> CreateUserAsync(CreateUserDTO createUserDto);
    public Task<string> LoginAsync(UserAccessDTO userAccessDto);
    public Task<UserDTO> GetUserAsync(string username);
    public Task<List<int>> GetFavoritesAsync(string username);
    public Task ChangeFavoriteAsync(ChangeFavoriteDTO request);
    public Task<UserDTO> EditUser(UserDTO editUserDto);
    
    public Task<List<String>> GetFavoriteUsersAsync(string username);
    public Task ChangeFavoriteUserAsync(ChangeFavoriteUserDTO request);

    public Task<List<UserDTO>> GetAllFavoritedBy(string username);

    public Task<List<CategoryDTO>> GetFavoriteCategoriesAsync(string username);
    public Task ChangeFavoriteCategoriesAsync(ChangeFavoriteCategories request);
    public Task<List<UserDTO>> GetUsersFavoritedByCategory(string categoryName);
}