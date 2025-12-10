using Contracts.WinningNumberDTOs;
using Microsoft.AspNetCore.Mvc;
using service.Services.Interfaces;
using Sieve.Models;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WinningNumberController : ControllerBase
{
    private readonly IWinningNumberService _winningNumberService;

    public WinningNumberController(IWinningNumberService winningNumberService)
    {
        _winningNumberService = winningNumberService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SieveModel? sieveModel)
    {
        var result = await _winningNumberService.GetAllAsync(sieveModel);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var winningNumber = await _winningNumberService.GetByIdAsync(id);
        return winningNumber == null ? NotFound() : Ok(winningNumber);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWinningNumberDto dto)
    {
        var created = await _winningNumberService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.WinningNumberID }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateWinningNumberDto dto)
    {
        var updated = await _winningNumberService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _winningNumberService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}