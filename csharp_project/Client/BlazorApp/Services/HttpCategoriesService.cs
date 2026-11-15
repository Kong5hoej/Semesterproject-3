using System.Text.Json;
using ApiContracts;

namespace BlazorApp.Services;

public class HttpCategoriesService : IHttpCategoriesService{
    private HttpClient client;

    public HttpCategoriesService(HttpClient client){
        this.client = client;
    }
    
    public async Task<List<CategoryDTO>> GetCategoriesAsync(){
        HttpResponseMessage httpResponse = await client.GetAsync($"categories");
        string response = await httpResponse.Content.ReadAsStringAsync();
        if (!httpResponse.IsSuccessStatusCode){
            throw new Exception(response);
        }

        return JsonSerializer.Deserialize<List<CategoryDTO>>(
            response, 
            new JsonSerializerOptions(){ PropertyNameCaseInsensitive = true }
        )!;
    }
}