using ApiContracts;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Services;

public class HttpImagesService : IHttpImagesService{
    private HttpClient client;
    public HttpImagesService(HttpClient client){
        this.client = client;
    }
    
    public string GetImageUrl(int imageId){
        return $"{client.BaseAddress!}images/{imageId}";
    }

    public async Task AddImagesToPostAsync(int postId, List<string> images){
        foreach (string image in images){
            HttpResponseMessage httpResponse =  await client.PostAsJsonAsync("images", new AddImageDTO(){ PostId = postId, Image = image });
            string response = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode){
                throw new Exception(response);
            }
        }
    }
}