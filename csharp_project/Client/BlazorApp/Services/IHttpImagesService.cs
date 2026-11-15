using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.Services;

public interface IHttpImagesService{
    string GetImageUrl(int imageId);
    Task AddImagesToPostAsync(int postId, List<string> images);
}