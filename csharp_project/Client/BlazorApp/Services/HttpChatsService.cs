using System.Net.Http.Headers;
using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpChatsService : IHttpChatsService{
    private HttpClient client;
    public HttpChatsService(HttpClient client){
        this.client = client;
    }

    public async Task<ChatDTO> MakeChatAsync(CreateChatDTO request)
    {
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync("chats", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<ChatDTO>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
        
    }

    public async Task<List<ChatDTO>> GetChatsAsync(int postId){
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{postId}/chat");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<ChatDTO>>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task<ChatDTO> GetChatAsync(int postId, string buyer){
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{postId}/chat/{buyer}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<ChatDTO>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }
    
    public async Task<ChatDTO> GetChatAsync(int chatId){
        HttpResponseMessage httpResponse = await client.GetAsync($"chats/{chatId}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<ChatDTO>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }
    public async Task<List<ChatDTO>> GetChatsAsync(string buyer)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"chats?buyer={buyer}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<ChatDTO>>(
        response, 
        new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }
}