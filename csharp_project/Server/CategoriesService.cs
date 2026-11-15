using Grpc.Net.Client;
using TradingForumGrpc;
using CategoryDTO = ApiContracts.CategoryDTO;

namespace Domain;

public class CategoriesService : ICategoriesService{
    private TradingForumGrpc.CategoryService.CategoryServiceClient Client{ get; set; }
    
    public CategoriesService(TradingForumGrpc.CategoryService.CategoryServiceClient client) {
        this.Client = client;
    }
    
    public async Task<List<CategoryDTO>> GetAllCategoriesAsync(){
        AllCategoriesResponse response = await Client.getAllCategoriesAsync(new Empty());

        List<CategoryDTO> categoryDtos = response.Categories
            .Select(categoryDto => new CategoryDTO(){
                Name = categoryDto.Name
            })
            .ToList();
        
        return categoryDtos;
    }

    public async Task<CategoryDTO> GetCategoryByNameAsync(string name)
    {
        AllCategoriesResponse response = await Client.getAllCategoriesAsync(new Empty());
        
        CategoryDTO categoryDto = response.Categories
            .Select(categoryDtoFromGrpc)
            .FirstOrDefault(c => c.Name.Equals(name))
            ?? throw new Exception("Category not found");
        return categoryDto;
    }
    
    private CategoryDTO categoryDtoFromGrpc(TradingForumGrpc.CategoryDTO categoryDto){
        return new CategoryDTO(){
            Name =  categoryDto.Name
        };
    }
}