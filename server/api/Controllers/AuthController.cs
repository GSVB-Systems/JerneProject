using api.Models;
using Contracts.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using service.Services;
using service.Services.Interfaces;
using service.Mappers;
using System.Security.Claims;


namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(TokenService tokenService, IAuthService authService, IUserService userService)
    {
        _tokenService = tokenService;
        _authService = authService;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var isValid = await _authService.verifyPasswordByEmailAsync(request.Username, request.Password);

        if (!isValid)
            return Unauthorized("Invalid credentials");

        var token = _tokenService.CreateToken(_authService.GetUserByEmailAsync(request.Username).Result);
       
        return Ok(new { token });
    }

    [HttpPost("User-change-password")]
    public async Task<IActionResult> UserChangePassword([FromQuery]string userId,[FromQuery] string oldPassword, [FromQuery] string newPassword)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            return BadRequest("All fields are required.");

        try
        {
            await _authService.UpdateUserPasswordAsync(userId, oldPassword, newPassword);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}