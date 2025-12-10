// csharp
using Microsoft.AspNetCore.Mvc;
using Contracts;
using Contracts.BoardDTOs;
using service.Services.Interfaces;
using Sieve.Models;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SieveModel? sieveModel)
        {
            var result = await _boardService.GetAllAsync(sieveModel);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var board = await _boardService.GetByIdAsync(id);
            return board == null ? NotFound() : Ok(board);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBoardDto dto)
        {
            var created = await _boardService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created?.BoardID }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateBoardDto dto)
        {
            var updated = await _boardService.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _boardService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}