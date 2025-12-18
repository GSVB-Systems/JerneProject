namespace api.Controllers;

using Contracts;
using Contracts.BoardDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Services.Interfaces;
using Sieve.Models;


[ApiController]
[Route("api/[controller]")]
public class BoardController : ControllerBase
{
    private readonly IBoardService _boardService;

    public BoardController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    [HttpGet("GetAllBoards")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAll([FromQuery] SieveModel? sieveModel)
    {
        var result = await _boardService.GetAllAsync(sieveModel);
        return Ok(result);
    }

    [HttpGet("GetBoardsById{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetById(string id)
    {
        var board = await _boardService.GetByIdAsync(id);
        return board == null ? NotFound() : Ok(board);
    }

    [HttpPost("CreateBoard")]
    [Authorize(Roles = "Bruger")]
    public async Task<IActionResult> Create([FromBody] CreateBoardDto dto)
    {
        var created = await _boardService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("UpdateBoard{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateBoardDto dto)
    {
        var updated = await _boardService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("DeleteBoard{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _boardService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("getAllBoardsByUserId")]
    [Authorize(Roles = "Administrator, Bruger")]
    public async Task<ActionResult<PagedResult<BoardDto>>> GetAllBoardsByUserId([FromQuery] string userId, [FromQuery] BoardQueryParameters parameters)
    {
        var boards = await _boardService.getAllByUserIdAsync(userId, parameters);
        return Ok(boards);
    }
}