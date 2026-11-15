using ApiContracts;
namespace BlazorApp.Services;

public interface IHttpPostsService{
    public Task<PostDTO> MakePostAsync(CreatePostDTO request);
    public Task ChangePostStateAsync(ChangePostStateDTO request);
    public Task ClosePostAsync(ClosePostDTO request);
    public Task<PostDTO> GetPost(int id);
    public Task<List<PostDTO>> GetPosts();
    public Task ChangePostAsync(PostDTO request, int id);
}