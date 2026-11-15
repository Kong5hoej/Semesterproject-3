using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpNotificationService : IHttpNotificationService
{
    private HttpClient client;
    
    public HttpNotificationService(HttpClient client){
        this.client = client;
    }
    
    public async Task<List<NotificationDTO>> GetNotifications(string username)
    {
        HttpResponseMessage httpResponse = await client.GetAsync($"user/{username}/notifications/");
        string response = await httpResponse.Content.ReadAsStringAsync();
        
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
        
        return JsonSerializer.Deserialize<List<NotificationDTO>>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }

    public async Task ReadNotifications(ReadNotificationsDTO readNotificationsDTO)
    {
        HttpResponseMessage httpResponse = await client.PatchAsJsonAsync($"user/{readNotificationsDTO.User}/notifications/", readNotificationsDTO);
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }
    }
}