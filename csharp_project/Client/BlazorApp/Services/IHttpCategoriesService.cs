using ApiContracts;

namespace BlazorApp.Services;

public interface IHttpCategoriesService{
    Task<List<CategoryDTO>> GetCategoriesAsync();
}