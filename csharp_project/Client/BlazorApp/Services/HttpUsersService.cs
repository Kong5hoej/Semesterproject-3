using System.Net;
using System.Text.Json;
using ApiContracts;
using ChangeFavoriteDTO=ApiContracts.ChangeFavoriteDTO;
using UserDTO=ApiContracts.UserDTO;

namespace BlazorApp.Services;

public class HttpUsersService : IHttpUsersService
{
    private HttpClient client;
    
    public HttpUsersService(HttpClient client){
        this.client = client;
    }
    
    public async Task<UserDTO> CreateUserAsync(CreateUserDTO request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("user", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            if(httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                throw new Exception("Brugeren er allerede oprettet");
        }

        return JsonSerializer.Deserialize<UserDTO>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }
    public async Task<UserDTO> getUserAsync(string username)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"user/{username}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<UserDTO>(
        response, 
        new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task<List<int>> GetFavoritesAsync(string username){
        HttpResponseMessage httpResponse = await client.GetAsync($"user/{username}/favorites");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<int>>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task ChangeFavoriteAsync(string username, int postId, bool favorite){
        ChangeFavoriteDTO request = new ChangeFavoriteDTO(){ User = username, PostId = postId, Favorite = favorite };
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync($"user/{username}/favorites", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
    }
    public async Task EditUserAsync(UserDTO request, string username)
    {
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync($"user/{username}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
    }
    public async Task<List<string>> GetFavoriteUsersAsync(string username)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"user/{username}/favoriteusers");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<String>>(
        response, 
        new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }
    public async Task ChangeFavoriteUserAsync(string username, string favoriteUser, bool favorite)
    {
        ChangeFavoriteUserDTO request = new ChangeFavoriteUserDTO(){
            Username = username, FavoriteUsername = favoriteUser, Favorite = favorite
        };
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync($"user/{username}/favoriteusers", request);
        
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
    }
    
    public async Task<List<CategoryDTO>> GetFavoriteCategoriesAsync(string username)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"user/{username}/favoritecategories");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<CategoryDTO>>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }
    
    public async Task ChangeFavoriteCategoriesAsync(string username, List<string> categories)
    {
        ChangeFavoriteCategories request = new ChangeFavoriteCategories(){
            Username = username, Categories = categories
        };
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync($"user/{username}/favoritecategories", request);
        
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
    }
}