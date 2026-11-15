using ApiContracts;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingForumGrpc;
using CreateChatDTO = ApiContracts.CreateChatDTO;
using PostDTO = ApiContracts.PostDTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PostsController : ControllerBase{
    private IPostsService postLogic;
    private IChatsService chatLogic;
    private IMessagesService messagesLogic;

    public PostsController(IPostsService postLogic, IChatsService chatLogic, IMessagesService messagesLogic){
        this.postLogic = postLogic;
        this.chatLogic = chatLogic;
        this.messagesLogic = messagesLogic;
    }

    [HttpPost(Name = "CreatePost")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO post){
        return Ok(await postLogic.MakePost(post));
    }

    [HttpPatch("changePostState", Name = "ChangePostState")]
    public async Task<IActionResult> ChangePostState([FromBody] ChangePostStateDTO changePostStateDto){
        await postLogic.ChangePostState(changePostStateDto);
        return Ok();
    }
    
    [HttpPatch("closePost", Name = "ClosePost")]
    public async Task<IActionResult> ClosePost([FromBody] ClosePostDTO closePostDto){
        await postLogic.ClosePost(closePostDto);
        return Ok();
    }

    [HttpPatch("{id:int}", Name = "EditPost")]
    public async Task<IActionResult> EditPost([FromBody] PostDTO post)
    {
        await postLogic.ChangePost(post);
        return Ok();
    }
    
    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetPost")]
    public async Task<IActionResult> GetPost([FromRoute] int id){
        return Ok(postLogic.GetPost(id));
    }
     
   [AllowAnonymous]
    [HttpGet(Name = "GetPosts")]
    public async Task<IActionResult> GetPosts(){
        return Ok(postLogic.GetPosts());
    }
    
    // -- Chat Here --
    [HttpGet("{postId:int}/chat/")]
    public async Task<IActionResult> GetChatsOnPost([FromRoute] int postId)
    {
        return Ok(await chatLogic.GetAllChatsAsync(postId, null));
    }
    
    [HttpGet("{postId:int}/chat/{username}")]
    public async Task<IActionResult> GetChat([FromRoute] string username,  [FromRoute] int postId)
    {
        ApiContracts.ChatDTO chat = await chatLogic.GetChat(username, postId);
        
        return Ok(chat);
    }
    
    // -- Mess Here --
    [HttpPost("{postId:int}/chat/{username}")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO sendMessageDto)
    {
        await messagesLogic.SendMessage(sendMessageDto);
        return Ok();
    }
}