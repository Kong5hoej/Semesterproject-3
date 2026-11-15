using ApiContracts;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController{
    private ICategoriesService service;

    public CategoriesController(ICategoriesService service){
        this.service = service;
    }

    [HttpGet(Name = "GetAllCategories")]
    public async Task<List<CategoryDTO>> GetAllCategoriesAsync(){
        return await service.GetAllCategoriesAsync();
    }
}