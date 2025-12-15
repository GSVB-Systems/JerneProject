using Contracts;
using dataaccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Contracts.TransactionDTOs;
using Microsoft.AspNetCore.Authorization;
using service.Services.Interfaces;
using Sieve.Models;

namespace api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
  
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SieveModel? sieveModel)
    {
        var result = await _transactionService.GetAllAsync(sieveModel);
        return Ok(result);
    }

    [HttpGet("getAllTransactionsByUserId")]
    [Authorize(Roles = "Administrator, Bruger")]
    public async Task<ActionResult<PagedResult<TransactionDto>>>  GetAllTransactionsByUserId([FromQuery] string userId, [FromQuery] TransactionQueryParameters parameters)
    {
        var transactions = await _transactionService.getAllByUserIdAsync(userId, parameters);
        return Ok(transactions);
    }

    [HttpGet("GetBy{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        return transaction == null ? NotFound() : Ok(transaction);
    }

    [HttpPost("CreateTransaction")]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto createDto)
    {
        var created = await _transactionService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = created.TransactionID }, created);
    }

    [HttpPut("UpdateTransactionBy{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTransactionDto dto)
    {
        var updated = await _transactionService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("DeleteTransactionBy{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _transactionService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

}