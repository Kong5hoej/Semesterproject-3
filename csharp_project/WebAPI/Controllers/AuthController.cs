using ApiContracts;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase{

    private readonly IUserService userService;

    public AuthController(IUserService userService)
    {
        this.userService = userService;
    }
    
    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserAccessDTO user)
    {
        return Ok(await userService.LoginAsync(user));
    }
    

}