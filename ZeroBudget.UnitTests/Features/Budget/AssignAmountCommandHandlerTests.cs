using FluentAssertions;
using NSubstitute;
using ZeroBudget.Application.Features.Budget.AssignAmount;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.Exceptions;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.UnitTests.Features.Budget;

public class AssignAmountCommandHandlerTests
{
    private readonly IBudgetEntryRepository _budgetEntries = Substitute.For<IBudgetEntryRepository>();
    private readonly ICategoryRepository _categories = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly AssignAmountCommandHandler _sut;

    private static readonly Guid CategoryId = Guid.NewGuid();
    private static readonly YearMonth Month = new(2025, 6);

    public AssignAmountCommandHandlerTests()
    {
        _sut = new AssignAmountCommandHandler(_budgetEntries, _categories, _uow);

        var cat = Category.Create(Guid.NewGuid(), "Food");
        _categories.GetByIdAsync(CategoryId).Returns(cat);

        _budgetEntries.GetAsync(CategoryId, Month).Returns((BudgetEntry?)null);

        _uow.ExecuteInTransactionAsync(Arg.Any<Func<Task>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.ArgAt<Func<Task>>(0)());
    }

    [Fact]
    public async Task Handle_NoExistingEntry_AddsNewEntry()
    {
        var command = new AssignAmountCommandInput(Month, CategoryId, 300m);
        await _sut.Handle(command, default);

        await _budgetEntries.Received(1).AddAsync(
            Arg.Is<BudgetEntry>(e => e.Assigned == 300m && e.CategoryId == CategoryId),
            default);
        await _uow.Received(1).SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_AssignExceedsAvailable_AllowsOverdraft()
    {
        var command = new AssignAmountCommandInput(Month, CategoryId, 100m);
        await _sut.Handle(command, default);

        await _budgetEntries.Received(1).AddAsync(
            Arg.Is<BudgetEntry>(e => e.Assigned == 100m),
            default);
    }

    [Fact]
    public async Task Handle_ExistingEntry_UpdatesAssigned()
    {
        var existing = BudgetEntry.Create(CategoryId, Month, 100m);
        _budgetEntries.GetAsync(CategoryId, Month).Returns(existing);

        var command = new AssignAmountCommandInput(Month, CategoryId, 250m);
        await _sut.Handle(command, default);

        existing.Assigned.Should().Be(250m);
        await _uow.Received(1).SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsNotFoundException()
    {
        _categories.GetByIdAsync(CategoryId).Returns((Category?)null);

        var command = new AssignAmountCommandInput(Month, CategoryId, 100m);
        var act = () => _sut.Handle(command, default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
