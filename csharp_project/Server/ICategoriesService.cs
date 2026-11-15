using ApiContracts;

namespace Domain;

public interface ICategoriesService{
    Task<List<CategoryDTO>> GetAllCategoriesAsync();
}