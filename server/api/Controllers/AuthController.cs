using Microsoft.AspNetCore.Mvc;
using api.Models;
using service.Models;
using service.Services;


namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // Example: hardcoded user — replace with real user validation
        if (request.Username != "admin" || request.Password != "password")
            return Unauthorized("Invalid credentials");

        var token = _tokenService.CreateToken(request.Username);
        return Ok(new { token });
    }
}