using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using ApiContracts;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TradingForumGrpc;
using CategoryDTO = ApiContracts.CategoryDTO;
using ChangeFavoriteDTO = TradingForumGrpc.ChangeFavoriteDTO;
using ChangeFavoriteUserDTO=ApiContracts.ChangeFavoriteUserDTO;
using PostDTO = ApiContracts.PostDTO;
using UserDTO = ApiContracts.UserDTO;

namespace Domain;

public class UserService : IUserService
{
    private TradingForumGrpc.UserService.UserServiceClient Client { get; set; }
    private IConfiguration Config { get; }

    public UserService(TradingForumGrpc.UserService.UserServiceClient client, IConfiguration config)
    {
        Client = client;
        Config = config;
    }

    public async Task<UserDTO> CreateUserAsync(CreateUserDTO createUserDto)
    {
        GetUsersResponse response = Client.getAllUsers(new Empty());
        if (response.User.Where(u => u.Username.Equals(createUserDto.Username)).FirstOrDefault() != null)
        {
            throw new Exception(
                "Der er allerede en bruger med dette navn");
        }
        
        CreateUserRequest request = new CreateUserRequest{Username = createUserDto.Username, Password = createUserDto.HashPassword, ZipCode = createUserDto.ZipCode};
        TradingForumGrpc.UserDTO userDto = await Client.createUserAsync(request); //Sender afsted til javaserver
        return new UserDTO{Username = userDto.Username, ZipCode = userDto.ZipCode};
    }

    public async Task<string> LoginAsync(UserAccessDTO userAccessDto)
    {
        LoginResponse response = await Client.loginAsync(new LoginRequest(){Username = userAccessDto.Username});
        if (response.Password == userAccessDto.Password)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userAccessDto.Username)
            };
            
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["JWT:Key"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: Config["JWT:Issuer"],
                audience: Config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        throw new UnauthorizedAccessException("Du har ikke adgang hertil!");
    }
    public async Task<UserDTO> GetUserAsync(string username)
    {
        GetUserRequest request = new GetUserRequest(){
            Username = username
        };

        TradingForumGrpc.UserDTO userDto = await Client.getUserAsync(request); //Sender afsted til javaserver
        return new UserDTO{Username = userDto.Username,  ZipCode = userDto.ZipCode};
    }

    public async Task<UserDTO> EditUser(UserDTO editUserDto)
    {
        if (String.IsNullOrEmpty(editUserDto.Username))
            throw new Exception("Brugernavn må ikke være tom!");

        TradingForumGrpc.UserDTO userDtoEdit = new TradingForumGrpc.UserDTO{
            Username = editUserDto.Username, ZipCode = editUserDto.ZipCode
        };

        TradingForumGrpc.UserDTO editedUser = Client.EditUser(userDtoEdit);
        
        return new UserDTO{Username = editedUser.Username, ZipCode = editedUser.ZipCode};
    }
 
    public async Task<List<int>> GetFavoritesAsync(string username){
        TradingForumGrpc.UserDTO request = new TradingForumGrpc.UserDTO(){
            Username = username
        };

        TradingForumGrpc.GetFavoritesResponse response = await Client.getFavoritesAsync(request); //Sender afsted til javaserver
        List<int> favorites = response.Ids.ToList();
        return favorites;
    }

    public async Task ChangeFavoriteAsync(ApiContracts.ChangeFavoriteDTO request){
        TradingForumGrpc.ChangeFavoriteDTO grpcRequest = new ChangeFavoriteDTO(){
            User = request.User,
            PostId = request.PostId,
            Favorite = request.Favorite
        };

        await Client.changeFavoriteAsync(grpcRequest);
    }
    
    public async Task<List<string>> GetFavoriteUsersAsync(string username)
    {
        TradingForumGrpc.GetFavoriteUsersRequest request = new TradingForumGrpc.GetFavoriteUsersRequest(){
            Username = username
        };
        
        GetFavoriteUsersResponse response = await Client.getFavoriteUsersAsync(request); //Sender afsted til javaserver
        List<string> favoriteUsers = response.FavoriteUsernames.ToList();
        return favoriteUsers;
    }
    
    public async Task ChangeFavoriteUserAsync(ChangeFavoriteUserDTO request)
    {
        TradingForumGrpc.ChangeFavoriteUserDTO grpcRequest = new TradingForumGrpc.ChangeFavoriteUserDTO(){
            Username = request.Username, Favorite = request.Favorite, FavoriteUsername = request.FavoriteUsername
        };
        await Client.changeFavoriteUserAsync(grpcRequest);
    }

    public async Task<List<UserDTO>> GetAllFavoritedBy(string username)
    {
        TradingForumGrpc.GetFavoritedByUsersRequest request = new TradingForumGrpc.GetFavoritedByUsersRequest() { Username = username };
        TradingForumGrpc.GetUsersResponse response = await Client.getFavoritedByAsync(request); 
        List<UserDTO> users = response.User
            .Select(userDtoFromGrpc)
            .ToList();
        return users;
    }

    public async Task<List<CategoryDTO>> GetFavoriteCategoriesAsync(string username)
    {
        TradingForumGrpc.GetFavoriteCategoriesRequest request = new TradingForumGrpc.GetFavoriteCategoriesRequest() { Username = username };
        TradingForumGrpc.GetFavoriteCategoriesResponse response = await Client.getFavoriteCategoriesAsync(request);
        List<CategoryDTO> categories = response.Categories
            .Select(categoryDtoFromGrpc)
            .ToList();
        return categories;
    }
    
    public async Task ChangeFavoriteCategoriesAsync(ChangeFavoriteCategories request)
    {
        Client.deleteAllFavoritedCategories(new TradingForumGrpc.DeleteFavoriteCategoriesRequest(){Username = request.Username});
        for (int i = 0; i < request.Categories.Count; i++)
        {
            TradingForumGrpc.ChangeFavoriteCategoryRequest grpcRequest = new TradingForumGrpc.ChangeFavoriteCategoryRequest(){
                Username = request.Username, 
                Category = request.Categories[i]
            };
            await Client.changeFavoriteCategoryAsync(grpcRequest);
        }
    }
    
    public async Task<List<UserDTO>> GetUsersFavoritedByCategory(string categoryName)
    {
        TradingForumGrpc.GetUsersFavoritedByCategoryRequest request = new TradingForumGrpc.GetUsersFavoritedByCategoryRequest() { CategoryName = categoryName};
        TradingForumGrpc.GetUsersFavoritedByCategoryResponse response = await Client.getUsersFavoritedByCategoryAsync(request); 
        List<UserDTO> users = response.Users
            .Select(userDtoFromGrpc)
            .ToList();
        return users;
    }
    
    private UserDTO userDtoFromGrpc(TradingForumGrpc.UserDTO userDto){
        return new UserDTO{
            Username = userDto.Username,
            ZipCode = userDto.ZipCode
        };
    }

    private CategoryDTO categoryDtoFromGrpc(TradingForumGrpc.CategoryDTO categoryDto)
    {
        return new CategoryDTO
        {
            Name = categoryDto.Name
        };
    }
}