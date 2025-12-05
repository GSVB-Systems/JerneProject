using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using service.Services.Interfaces;
using Contracts.UserDTOs;


namespace api.Controllers;



[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    
    [HttpGet("getAll")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(RegisterUserDto dto)
    {
        var created = await _userService.RegisterUserAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.UserID }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UserDto dto)
    {
        var updated = await _userService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _userService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}