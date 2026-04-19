using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZeroBudget.Application.Common;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Features.Transactions.CreateTransaction;
using ZeroBudget.Application.Features.Transactions.DeleteTransaction;
using ZeroBudget.Application.Features.Transactions.GetTransactions;
using ZeroBudget.Application.Features.Transactions.UpdateTransaction;

namespace ZeroBudget.Api.Controllers;

[Route("api/transactions")]
public class TransactionsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetTransactionsQueryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? accountId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        if (accountId is null)
            return BadRequest("accountId is required.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = fromDate ?? new DateOnly(today.Year, today.Month, 1);
        var to = toDate ?? new DateOnly(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

        var result = await mediator.Send(
            new GetTransactionsQueryInput(accountId.Value, from, to, search, page, pageSize), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateTransactionCommandOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateTransactionCommandInput(
                request.AccountId,
                request.Amount,
                request.Date,
                request.CategoryId,
                request.CategoryName,
                request.CategoryGroupId,
                request.Memo,
                request.AffectsBudget),
            ct);
        return CreatedAtAction(nameof(GetAll), new { accountId = result.Transaction.AccountId }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateTransactionCommandOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTransactionRequest request,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateTransactionCommandInput(
                id,
                request.Amount,
                request.Date,
                request.CategoryId,
                request.Memo,
                request.AffectsBudget),
            ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteTransactionCommandInput(id), ct);
        return NoContent();
    }
}

public class CreateTransactionRequest
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? CategoryGroupId { get; set; }
    public string? Memo { get; set; }
    public bool AffectsBudget { get; set; } = true;
}

public class UpdateTransactionRequest
{
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Memo { get; set; }
    public bool AffectsBudget { get; set; } = true;
}
