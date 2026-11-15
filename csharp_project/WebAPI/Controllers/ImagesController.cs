using ApiContracts;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ImagesController : ControllerBase{
    private IImagesService imagesService;

    public ImagesController(IImagesService imagesService){
        this.imagesService = imagesService;
    }

    [HttpGet("{id:int}", Name = "GetImage")]
    public async Task<IActionResult> GetImage([FromRoute] int id){
        ImageDTO imageDto = await imagesService.GetImageAsync(id);
        var response = Ok(imageDto.Data);
        string contentType = "";
        if (imagesService.IsJpeg(imageDto.Data))
            contentType = "image/jpeg";
        else if (imagesService.IsPng(imageDto.Data))
            contentType = "image/png";
        else if (imagesService.IsWebp(imageDto.Data))
            contentType = "image/webp";

        if (contentType == ""){
            return Problem();
        }
        
        return File(imageDto.Data, contentType);
    }

    [HttpPost(Name = "AddImage")]
    [Authorize]
    public async Task<IActionResult> AddImage([FromBody] AddImageDTO addImageDto){
        await imagesService.AddImageAsync(addImageDto.PostId, Convert.FromBase64String(addImageDto.Image));
        return Ok();
    }
}