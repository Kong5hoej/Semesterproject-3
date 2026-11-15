using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ChatsController :ControllerBase{
    private IChatsService chatLogic;

    public ChatsController(IChatsService chatLogic){
        this.chatLogic = chatLogic;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetChats([FromQuery] int? postId, [FromQuery] string? buyer)
    {
        return Ok(await chatLogic.GetAllChatsAsync(postId, buyer));
    }
    
    [HttpGet("{chatId:int}")]
    public async Task<IActionResult> GetChat([FromRoute] int chatId)
    {
        ApiContracts.ChatDTO chat = chatLogic.GetChat(chatId);
        
        return Ok(chat);
    }
    
}