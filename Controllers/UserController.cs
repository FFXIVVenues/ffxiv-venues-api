using FFXIVVenues.Api.Security.UserAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FFXIVVenues.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        this._userService = userService;
    }
    
    [Authorize]
    [HttpGet("@me")]
    public ActionResult<User> Me()
    {
        var user = this._userService.GetCurrentUser();
        if (user == null)
            return this.Challenge();
        
        return user;
    }
}
