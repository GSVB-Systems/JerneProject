using Contracts;
using Contracts.WinningBoardDTOs;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("GetAllWinningBoards")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<PagedResult<WinningBoardDto>>> GetAll([FromQuery] SieveModel? sieveModel)
    {
        var result = await _winningBoardService.GetAllAsync(sieveModel);
        return Ok(result);
    }

    [HttpGet("GetWinningBoardBy{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<WinningBoardDto>> GetById(string id)
    {
        var winningBoard = await _winningBoardService.GetByIdAsync(id);
        return winningBoard == null ? NotFound() : Ok(winningBoard);
    }

    [HttpPost("CreateWinningBoard")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<WinningBoardDto>> Create([FromBody] CreateWinningBoardDto dto)
    {
        var created = await _winningBoardService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("UpdateWinningBoardBy{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<WinningBoardDto>> Update(string id, [FromBody] UpdateWinningBoardDto dto)
    {
        var updated = await _winningBoardService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("DeleteWinningBoardBy{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> Delete(string id)
    {
        var deleted = await _winningBoardService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}