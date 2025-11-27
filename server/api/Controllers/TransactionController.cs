using dataaccess.Entities;
using Microsoft.AspNetCore.Mvc;
using service.Services.Interfaces;

namespace api.Controllers;

public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
  
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _transactionService.GetAllAsync();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        return transaction == null ? NotFound() : Ok(transaction);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Transaction transaction)
    {
        var created = await _transactionService.CreateAsync(transaction);
        return CreatedAtAction(nameof(GetById), new { id = created.TransactionID }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Transaction transaction)
    {
        var updated = await _transactionService.UpdateAsync(id, transaction);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _transactionService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

}