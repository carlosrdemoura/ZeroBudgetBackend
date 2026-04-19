using FluentAssertions;
using NSubstitute;
using ZeroBudget.Application.Features.Transactions.CreateTransaction;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.UnitTests.Features.Transactions;

public class CreateTransactionCommandHandlerTests
{
    private readonly ITransactionRepository _transactions = Substitute.For<ITransactionRepository>();
    private readonly ICategoryRepository _categories = Substitute.For<ICategoryRepository>();
    private readonly IAccountRepository _accounts = Substitute.For<IAccountRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly CreateTransactionCommandHandler _sut;

    private static readonly Guid AccountId = Guid.NewGuid();
    private static readonly Guid CategoryId = Guid.NewGuid();
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    public CreateTransactionCommandHandlerTests()
    {
        _sut = new CreateTransactionCommandHandler(_transactions, _categories, _accounts, _uow);

        var account = Account.Create("Checking");
        _accounts.GetByIdAsync(AccountId).Returns(account);
    }

    [Fact]
    public async Task Handle_PositiveAmount_NoCategoryRequired()
    {
        var command = new CreateTransactionCommandInput(AccountId, 2000m, Today, null, null, null, "Salary");

        var result = await _sut.Handle(command, default);

        result.Transaction.Amount.Should().Be(2000m);
        result.Transaction.CategoryId.Should().BeNull();
        await _transactions.Received(1).AddAsync(Arg.Any<Transaction>(), default);
        await _uow.Received(1).SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_NegativeAmountWithValidCategory_CreatesTransaction()
    {
        var category = Category.Create(Guid.NewGuid(), "Groceries");
        _categories.GetByIdAsync(CategoryId).Returns(category);

        var command = new CreateTransactionCommandInput(AccountId, -75m, Today, CategoryId, null, null, "Weekly shop");

        var result = await _sut.Handle(command, default);

        result.Transaction.Amount.Should().Be(-75m);
        result.Transaction.CategoryId.Should().Be(CategoryId);
        result.Transaction.CategoryName.Should().Be("Groceries");
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ThrowsNotFoundException()
    {
        _categories.GetByIdAsync(CategoryId).Returns((Category?)null);

        var command = new CreateTransactionCommandInput(AccountId, -50m, Today, CategoryId, null, null, null);

        var act = () => _sut.Handle(command, default);

        await act.Should().ThrowAsync<NotFoundException>();
        await _transactions.DidNotReceive().AddAsync(Arg.Any<Transaction>(), default);
    }

    [Fact]
    public async Task Handle_PersistsTransaction()
    {
        var command = new CreateTransactionCommandInput(AccountId, 500m, Today, null, null, null, null);
        await _sut.Handle(command, default);

        await _uow.Received(1).SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_AffectsBudgetFalse_ForwardsFlagToTransaction()
    {
        var command = new CreateTransactionCommandInput(AccountId, 250m, Today, null, null, null, "Info only", affectsBudget: false);

        var result = await _sut.Handle(command, default);

        result.Transaction.AffectsBudget.Should().BeFalse();
        await _transactions.Received(1).AddAsync(
            Arg.Is<Transaction>(t => t.AffectsBudget == false),
            default);
    }

    [Fact]
    public async Task Handle_NewCategoryNameWithGroupId_CreatesCategoryInGivenGroup()
    {
        var group = CategoryGroup.Create("Needs");
        _categories.FindByNameAsync("Utilities").Returns((Category?)null);
        _categories.GetGroupByIdAsync(group.Id).Returns(group);

        var command = new CreateTransactionCommandInput(AccountId, -120m, Today, null, "Utilities", group.Id, null);

        var result = await _sut.Handle(command, default);

        result.Transaction.CategoryName.Should().Be("Utilities");
        await _categories.Received(1).AddCategoryAsync(
            Arg.Is<Category>(c => c.GroupId == group.Id && c.Name == "Utilities"),
            default);
        await _categories.DidNotReceive().GetFirstUserGroupAsync(default);
    }

    [Fact]
    public async Task Handle_NewCategoryNameWithMissingGroupId_ThrowsNotFoundException()
    {
        var missingGroupId = Guid.NewGuid();
        _categories.FindByNameAsync("Utilities").Returns((Category?)null);
        _categories.GetGroupByIdAsync(missingGroupId).Returns((CategoryGroup?)null);

        var command = new CreateTransactionCommandInput(AccountId, -120m, Today, null, "Utilities", missingGroupId, null);

        var act = () => _sut.Handle(command, default);

        await act.Should().ThrowAsync<NotFoundException>();
        await _categories.DidNotReceive().AddCategoryAsync(Arg.Any<Category>(), default);
    }
}
