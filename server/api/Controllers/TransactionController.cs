using Contracts.BoardDTOs;

namespace api.Controllers;

using Contracts;
using Contracts.TransactionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Services.Interfaces;
using Sieve.Models;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IPurchaseService _purchaseService;
  
    public TransactionController(ITransactionService transactionService, IPurchaseService purchaseService)
    {
        _transactionService = transactionService;
        _purchaseService = purchaseService;
        
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SieveModel? sieveModel)
    {
        var result = await _transactionService.GetAllAsync(sieveModel);
        return Ok(result);
    }

    [HttpGet("getAllTransactionsByUserId")]
    [Authorize(Roles = "Administrator, Bruger")]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetAllTransactionsByUserId([FromQuery] string userId, [FromQuery] TransactionQueryParameters parameters)
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
        return Ok(created);
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

    [HttpPost("Purchase")]
    [Authorize(Roles = "Administrator, Bruger")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseDTO purchaseDto )
    {
        var result = await _purchaseService.ProcessPurchaseAsync(purchaseDto.Board, purchaseDto.Transaction);
        return result ? Ok("Purchase completed successfully.") : BadRequest("Purchase failed.");
    }

}