using dataaccess.Entities;
using Microsoft.AspNetCore.Mvc;
using service.Services.Interfaces;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WinningNumberController : ControllerBase
{
    private readonly IWinningNumberService _winningNumberService;
    
    public WinningNumberController(IWinningNumberService winningNumberService)
    {
        _winningNumberService = winningNumberService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var winningNumbers = await _winningNumberService.GetAllAsync();
        return Ok(winningNumbers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var winningNumber = await _winningNumberService.GetByIdAsync(id);
        return winningNumber == null ? NotFound() : Ok(winningNumber);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(WinningNumber winningBoard)
    {
        var created = await _winningNumberService.CreateAsync(winningBoard);
        return CreatedAtAction(nameof(GetById), new { id = created.WinningNumberID }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, WinningNumber winningNumber)
    {
        var updated = await _winningNumberService.UpdateAsync(id, winningNumber);
        return updated == null ? NotFound() : Ok(updated);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _winningNumberService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}