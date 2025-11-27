using dataaccess.Entities;
using Microsoft.AspNetCore.Mvc;
using service.Services.Interfaces;

namespace api.Controllers;

public class WinningBoardController : ControllerBase
{
    
    private readonly IWinningBoardService _winningBoardService;
    
    public WinningBoardController(IWinningBoardService winningBoardService)
    {
        _winningBoardService = winningBoardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var winningBoards = await _winningBoardService.GetAllAsync();
        return Ok(winningBoards);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var winningBoard = await _winningBoardService.GetByIdAsync(id);
        return winningBoard == null ? NotFound() : Ok(winningBoard);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(WinningBoard winningBoard)
    {
        var created = await _winningBoardService.CreateAsync(winningBoard);
        return CreatedAtAction(nameof(GetById), new { id = created.WinningBoardID }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, WinningBoard winningBoard)
    {
        var updated = await _winningBoardService.UpdateAsync(id, winningBoard);
        return updated == null ? NotFound() : Ok(updated);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _winningBoardService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

}