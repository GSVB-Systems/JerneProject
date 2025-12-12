using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Contracts;
using Contracts.BoardDTOs;
using Microsoft.AspNetCore.Authorization;
using Sieve.Models;
using service.Services.Interfaces;

namespace service.Controllers
{
    [AllowAnonymous]
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
        public async Task<ActionResult<List<string>>> GetBoardsContainingNumbers(string winningBoardId)
        {
            if (string.IsNullOrWhiteSpace(winningBoardId))
                return BadRequest();

            var matches = await _boardMatcherService.GetBoardsContainingNumbersAsync(winningBoardId);

            if (matches == null || !matches.Any())
                return NotFound();

            return Ok(matches);
        }
    }
}