using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using service.Services;
using service.Services.Interfaces;


namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly IAuthService _authService;

    public AuthController(TokenService tokenService, IAuthService authService)
    {
        _tokenService = tokenService;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var isValid = await _authService.verifyPasswordByEmailAsync(request.Username, request.Password);

        if (!isValid)
            return Unauthorized("Invalid credentials");

        var token = _tokenService.CreateToken(_authService.GetUserByEmailAsync(request.Username).Result);
        return Ok(new { token });
    }
}