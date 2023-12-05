using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ExampleController : ControllerBase
{
    [HttpGet(Name = "Public")]
    public IActionResult PublicAction()
    {
        return Ok("Hello from public action.");
    }

    [HttpGet(Name = "Logged")]
    [Authorize]
    public IActionResult LoggedAction()
    {
        return Ok("Hello from logged action.");
    }

    [HttpGet(Name = "Admin")]
    [Authorize(Roles = "Administrator")]
    public IActionResult AdminAction()
    {
        return Ok("Hello from admin action.");
    }
}
