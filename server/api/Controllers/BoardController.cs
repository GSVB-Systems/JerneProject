// csharp
using dataaccess.Entities;
using service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Reflection;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardsController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public BoardsController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var boards = await _boardService.GetAllAsync();
            return Ok(boards);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var board = await _boardService.GetByIdAsync(id);
            return board == null ? NotFound() : Ok(board);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Board board)
        {
            var created = await _boardService.CreateAsync(board);

            object? idValue = null;
            if (created != null)
            {
                var type = created.GetType();
                var prop = type.GetProperty("Id") ?? type.GetProperty("BoardID") ?? type.GetProperty("BoardId");
                idValue = prop?.GetValue(created);
            }

            return CreatedAtAction(nameof(GetById), new { id = idValue }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Board board)
        {
            var updated = await _boardService.UpdateAsync(id, board);
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