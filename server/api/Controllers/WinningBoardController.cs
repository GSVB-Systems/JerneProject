using Contracts.WinningBoardDTOs;
using Microsoft.AspNetCore.Mvc;
using service.Services.Interfaces;
using Sieve.Models;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WinningBoardController : ControllerBase
{
    private readonly IWinningBoardService _winningBoardService;

    public WinningBoardController(IWinningBoardService winningBoardService)
    {
        _winningBoardService = winningBoardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SieveModel? sieveModel)
    {
        var result = await _winningBoardService.GetAllAsync(sieveModel);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var winningBoard = await _winningBoardService.GetByIdAsync(id);
        return winningBoard == null ? NotFound() : Ok(winningBoard);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWinningBoardDto dto)
    {
        var created = await _winningBoardService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.WinningBoardID }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateWinningBoardDto dto)
    {
        var updated = await _winningBoardService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _winningBoardService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}