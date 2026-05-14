using FluentAssertions;
using NSubstitute;
using ZeroBudget.Application.Features.Transactions.CreateTransaction;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.UnitTests.Features.Transactions;

public class CreateTransactionCommandHandlerTests
{
    private readonly ITransactionRepository _transactions = Substitute.For<ITransactionRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly CreateTransactionCommandHandler _sut;

    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    public CreateTransactionCommandHandlerTests()
    {
        _sut = new CreateTransactionCommandHandler(_transactions, _uow);
    }

    [Fact]
    public async Task Handle_PositiveAmount_PersistsAsReceivable()
    {
        var command = new CreateTransactionCommandInput(2000m, Today, "Salary", isConsolidated: true);

        var result = await _sut.Handle(command, default);

        result.Transaction.Amount.Should().Be(2000m);
        result.Transaction.Description.Should().Be("Salary");
        result.Transaction.IsConsolidated.Should().BeTrue();
        await _transactions.Received(1).AddAsync(Arg.Any<Transaction>(), default);
        await _uow.Received(1).SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_NegativeAmount_PersistsAsPayable()
    {
        var command = new CreateTransactionCommandInput(-75m, Today, "Rent", isConsolidated: false);

        var result = await _sut.Handle(command, default);

        result.Transaction.Amount.Should().Be(-75m);
        result.Transaction.IsConsolidated.Should().BeFalse();
        await _transactions.Received(1).AddAsync(
            Arg.Is<Transaction>(t => t.Amount == -75m && t.IsConsolidated == false),
            default);
    }

    [Fact]
    public async Task Handle_AlwaysCallsSaveChanges()
    {
        var command = new CreateTransactionCommandInput(500m, Today, null, isConsolidated: false);
        await _sut.Handle(command, default);

        await _uow.Received(1).SaveChangesAsync(default);
    }
}
