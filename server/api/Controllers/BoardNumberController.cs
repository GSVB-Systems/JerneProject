using dataaccess.Entities;
using Microsoft.AspNetCore.Mvc;
using service.Services.Interfaces;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardNumberController : ControllerBase
{
    private readonly IBoardNumberService _boardNumberService;
    
    public BoardNumberController(IBoardNumberService boardNumberService)
    {
        _boardNumberService = boardNumberService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var boardNumbers = await _boardNumberService.GetAllAsync();
        return Ok(boardNumbers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var boardNumber = await _boardNumberService.GetByIdAsync(id);
        return boardNumber == null ? NotFound() : Ok(boardNumber);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(BoardNumber boardNumber)
    {
        var created = await _boardNumberService.CreateAsync(boardNumber);
        return CreatedAtAction(nameof(GetById), new { id = created.BoardNumberID }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, BoardNumber boardNumber)
    {
        var updated = await _boardNumberService.UpdateAsync(id, boardNumber);
        return updated == null ? NotFound() : Ok(updated);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _boardNumberService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}