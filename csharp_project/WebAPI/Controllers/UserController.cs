using ApiContracts;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChangeFavoriteDTO=ApiContracts.ChangeFavoriteDTO;
using UserDTO=ApiContracts.UserDTO;
namespace WebAPI.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase{
    private IUserService userService;
    private INotificationService notificationService;

    public UserController(IUserService userService, INotificationService notificationService)
    {
        this.userService = userService;
        this.notificationService =  notificationService;
    }
    
    [AllowAnonymous]
    [HttpPost(Name = "CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO user){
        return Ok(await userService.CreateUserAsync(user));
    }

    [AllowAnonymous]
    [HttpGet("{username}",Name = "GetUser")]
    public async Task<IActionResult> GetUser([FromRoute] string username)
    {
        return Ok(await userService.GetUserAsync(username));
    }
    
    [HttpGet("{username}/favorites",Name = "GetFavorites")]
    public async Task<IActionResult> GetFavorites([FromRoute] string username)
    {
        return Ok(await userService.GetFavoritesAsync(username));
    }
    
    [HttpPatch("{username}/favorites", Name = "ChangeFavorite")]
    public async Task<IActionResult> ChangeFavorite([FromBody] ChangeFavoriteDTO request){
        await userService.ChangeFavoriteAsync(request);
        return Ok();
    }

    [HttpPatch("{username}", Name = "EditUser")]
    public async Task<IActionResult> EditUser([FromBody] UserDTO user)
    {
        return Ok(await userService.EditUser(user));
    }
    
    [HttpGet("{username}/notifications")]
    public async Task<IActionResult> GetNotifications([FromRoute] string username)
    {
        return Ok(notificationService.GetNotifications(username));
    }
    
    [HttpPatch("{username}/notifications")]
    public async Task<IActionResult> ReadNotifications([FromBody] ReadNotificationsDTO readNotificationsDTO)
    {
        notificationService.ReadNotifications(readNotificationsDTO);
        return Ok();
    }

    [HttpGet("{username}/favoriteusers", Name = "GetFavoriteUsers")]
    public async Task<IActionResult> GetFavoriteUsers([FromRoute] string username)
    {
        return Ok(await userService.GetFavoriteUsersAsync(username));
    }

    [HttpPatch("{username}/favoriteusers", Name = "ChangeFavoriteUsers")]
    public async Task<IActionResult> ChangeFavoriteUsers([FromBody] ChangeFavoriteUserDTO request)
    {
        await userService.ChangeFavoriteUserAsync(request);
        return Ok();
    }
    
    [HttpGet("{username}/favoritecategories", Name = "GetFavoriteCategories")]
    public async Task<IActionResult> GetFavoriteCategories([FromRoute] string username)
    {
        return Ok(await userService.GetFavoriteCategoriesAsync(username));
    }

    [HttpPatch("{username}/favoritecategories", Name = "GetFavoriteCategories")]
    public async Task<IActionResult> ChangeFavoriteCategories([FromBody] ChangeFavoriteCategories request)
    {
        await userService.ChangeFavoriteCategoriesAsync(request);
        return Ok();
    }
}