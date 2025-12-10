using dataaccess.Entities;
using Microsoft.AspNetCore.Mvc;
using service.Services.Interfaces;
using Sieve.Models;
using Contracts.BoardNumberDTOs;

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
    public async Task<IActionResult> GetAll([FromQuery] SieveModel? sieveModel)
    {
        var result = await _boardNumberService.GetAllAsync(sieveModel);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var boardNumber = await _boardNumberService.GetByIdAsync(id);
        return boardNumber == null ? NotFound() : Ok(boardNumber);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBoardNumberDto dto)
    {
        var created = await _boardNumberService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.BoardNumberID }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBoardNumberDto dto)
    {
        var updated = await _boardNumberService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _boardNumberService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}