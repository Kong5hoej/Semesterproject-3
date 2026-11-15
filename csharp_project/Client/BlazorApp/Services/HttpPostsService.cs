using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpPostsService : IHttpPostsService{
    private HttpClient client;
    
    public HttpPostsService(HttpClient client){
        this.client = client;
    }

    public async Task<PostDTO> MakePostAsync(CreatePostDTO request){
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("posts", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<PostDTO>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task ChangePostStateAsync(ChangePostStateDTO request){
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync("posts/changePostState", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
    }

    public async Task ClosePostAsync(ClosePostDTO request){
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync("posts/closePost", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
    }

    public async Task<PostDTO> GetPost(int id){
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{id}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<PostDTO>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task<List<PostDTO>> GetPosts(){
        HttpResponseMessage httpResponse = await client.GetAsync($"posts");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<PostDTO>>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }
    
    public async Task ChangePostAsync(PostDTO request, int id)
    {
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync($"posts/{id}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
    }
}