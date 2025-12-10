using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using service.Services.Interfaces;
using Contracts.UserDTOs;
using Contracts;


namespace api.Controllers;



[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("getAll")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAll([FromQuery] UserQueryParameters parameters)
    {
        var users = await _userService.GetAllAsync(parameters);
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
        var created = await _userService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateUserDto dto)
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
    
    [HttpGet("{id}/subscription")]
    [Authorize]
    public async Task<IActionResult> IsSubscriptionActive(string id)
    {
        var isActive = await _userService.IsSubscriptionActiveAsync(id);
        return Ok(new { isActive });
    }
    
    [HttpPost("{id}/extend")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ExtendSubscription(string id, [FromQuery] int months)
    {
        if (months <= 0) return BadRequest("Months must be greater than zero.");
        var updated = await _userService.ExtendSubscriptionAsync(id, months);
        return updated == null ? NotFound() : Ok(updated);
    }
}