using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZeroBudget.Application.Features.Budget.AssignAmount;
using ZeroBudget.Application.Features.Budget.GetMonthSummary;
using ZeroBudget.Application.Features.Budget.MoveMoney;
using ZeroBudget.Application.Features.Categories.GetCategoryBalances;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Api.Controllers;

[Route("api/budget")]
public class BudgetController(IMediator mediator) : ApiControllerBase
{
    [HttpGet("{month}")]
    [ProducesResponseType(typeof(GetMonthSummaryQueryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMonthSummary(string month, CancellationToken ct)
    {
        if (!TryParseMonth(month, out var yearMonth))
            return BadRequest("Month must be in YYYY-MM format.");

        var result = await mediator.Send(new GetMonthSummaryQueryInput(yearMonth), ct);
        return Ok(result);
    }

    [HttpGet("{month}/balances")]
    [ProducesResponseType(typeof(GetCategoryBalancesQueryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategoryBalances(string month, CancellationToken ct)
    {
        if (!TryParseMonth(month, out var yearMonth))
            return BadRequest("Month must be in YYYY-MM format.");

        var result = await mediator.Send(new GetCategoryBalancesQueryInput(yearMonth), ct);
        return Ok(result);
    }

    [HttpPut("{month}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignAmount(
        string month,
        [FromBody] AssignAmountRequest request,
        CancellationToken ct)
    {
        if (!TryParseMonth(month, out var yearMonth))
            return BadRequest("Month must be in YYYY-MM format.");

        await mediator.Send(
            new AssignAmountCommandInput(yearMonth, request.CategoryId, request.Amount), ct);
        return NoContent();
    }

    [HttpPost("{month}/move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MoveMoney(
        string month,
        [FromBody] MoveMoneyRequest request,
        CancellationToken ct)
    {
        if (!TryParseMonth(month, out var yearMonth))
            return BadRequest("Month must be in YYYY-MM format.");

        await mediator.Send(
            new MoveMoneyCommandInput(yearMonth, request.FromCategoryId, request.ToCategoryId, request.Amount),
            ct);
        return NoContent();
    }

    private static bool TryParseMonth(string value, out YearMonth result)
    {
        try
        {
            result = YearMonth.Parse(value);
            return true;
        }
        catch
        {
            result = default!;
            return false;
        }
    }
}

public class AssignAmountRequest
{
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
}

public class MoveMoneyRequest
{
    public Guid FromCategoryId { get; set; }
    public Guid ToCategoryId { get; set; }
    public decimal Amount { get; set; }
}
