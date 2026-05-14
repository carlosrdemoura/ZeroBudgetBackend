using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZeroBudget.Application.Features.Transactions.ConsolidateTransaction;
using ZeroBudget.Application.Features.Transactions.CreateTransaction;
using ZeroBudget.Application.Features.Transactions.DeleteTransaction;
using ZeroBudget.Application.Features.Transactions.GetTransactions;
using ZeroBudget.Application.Features.Transactions.UpdateTransaction;
using ZeroBudget.Application.Features.Transactions.UpdateTransactionPosition;

namespace ZeroBudget.Api.Controllers;

[Route("api/transactions")]
public class TransactionsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetTransactionsQueryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var resolvedYear = year ?? today.Year;
        var resolvedMonth = month ?? today.Month;

        var result = await mediator.Send(
            new GetTransactionsQueryInput(resolvedYear, resolvedMonth, search, page, pageSize), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateTransactionCommandOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateTransactionCommandInput(
                request.Amount,
                request.Date,
                request.Description,
                request.IsConsolidated,
                request.Position),
            ct);
        return CreatedAtAction(nameof(GetAll), new { year = result.Transaction.Date.Year, month = result.Transaction.Date.Month }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateTransactionCommandOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                request.Description,
                request.IsConsolidated),
            ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/consolidate")]
    [ProducesResponseType(typeof(ConsolidateTransactionCommandOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Consolidate(
        Guid id,
        [FromBody] ConsolidateTransactionRequest request,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new ConsolidateTransactionCommandInput(id, request.IsConsolidated), ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/position")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePosition(
        Guid id,
        [FromBody] UpdateTransactionPositionRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new UpdateTransactionPositionCommandInput(id, request.Position), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteTransactionCommandInput(id), ct);
        return NoContent();
    }
}

public class CreateTransactionRequest
{
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
    public bool IsConsolidated { get; set; }
    public double? Position { get; set; }
}

public class UpdateTransactionRequest
{
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
    public bool IsConsolidated { get; set; }
}

public class ConsolidateTransactionRequest
{
    public bool IsConsolidated { get; set; }
}

public class UpdateTransactionPositionRequest
{
    public double Position { get; set; }
}
