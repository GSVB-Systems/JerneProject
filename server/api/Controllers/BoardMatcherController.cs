using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using service.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Contracts.WinnerResultDTO;

namespace service.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class BoardMatcherController : ControllerBase
    {
        private readonly IBoardMatcherService _boardMatcherService;

        public BoardMatcherController(IBoardMatcherService boardMatcherService)
        {
            _boardMatcherService = boardMatcherService;
        }

        [HttpGet("FindAllBoardsMathingWinningBoardID/{winningBoardId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<WinnerResultDto>>> GetBoardsContainingNumbers(string winningBoardId)
        {
            if (string.IsNullOrWhiteSpace(winningBoardId))
                return BadRequest();

            var matches = await _boardMatcherService.GetBoardsContainingNumbersAsync(winningBoardId);

            if (matches == null || !matches.Any())
                return NotFound();

            return Ok(matches);
        }
        
        [HttpGet("FindAllBoardsMathingWinningBoardIDWithDecrementer/{winningBoardId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<List<WinnerResultDto>>> GetBoardsContainingNumbersWithDecrementer(string winningBoardId)
        {
            if (string.IsNullOrWhiteSpace(winningBoardId))
                return BadRequest();

            var matches = await _boardMatcherService.GetBoardsContainingNumbersWithDecrementerAsync(winningBoardId);

            if (matches == null || !matches.Any())
                return NotFound();

            return Ok(matches);
        }

    }
}