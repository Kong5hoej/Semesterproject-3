using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpMessagesService :  IHttpMessagesService
{
    private HttpClient client;

    public HttpMessagesService(HttpClient client){
        this.client = client;
    }

    public async Task SendMessage(int postId, string buyer, MessageDTO message){
        SendMessageDTO request = new SendMessageDTO(){ PostId = postId, Buyer = buyer, Message = message };
        HttpResponseMessage httpResponse = await client.PostAsJsonAsync($"posts/{postId}/chat/{buyer}", request);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
    }

    public async Task<List<MessageDTO>> GetMessagesAsync(int postId, int chatId){
        HttpResponseMessage httpResponse = await client.GetAsync($"posts/{postId}/chat/{chatId}");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<MessageDTO>>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }
}