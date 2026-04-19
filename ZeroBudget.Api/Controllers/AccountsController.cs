using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZeroBudget.Application.Features.Accounts.CreateAccount;
using ZeroBudget.Application.Features.Accounts.DeleteAccount;
using ZeroBudget.Application.Features.Accounts.GetAccounts;
using ZeroBudget.Application.Features.Accounts.GetAccountsSummary;
using ZeroBudget.Application.Features.Accounts.UpdateAccount;

namespace ZeroBudget.Api.Controllers;

[Route("api/accounts")]
public class AccountsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetAccountsQueryOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAccountsQueryInput(), ct);
        return Ok(result);
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(GetAccountsSummaryQueryOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAccountsSummaryQueryInput(), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateAccountCommandOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateAccountCommandInput(request.Name, request.InitialBalance), ct);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateAccountCommandOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateAccountCommandInput(id, request.Name, request.CurrentBalance), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteAccountCommandInput(id), ct);
        return NoContent();
    }
}

public class CreateAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; }
}

public class UpdateAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
}
