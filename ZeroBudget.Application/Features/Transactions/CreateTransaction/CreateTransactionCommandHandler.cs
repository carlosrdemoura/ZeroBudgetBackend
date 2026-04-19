using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTransactionCommandInput, CreateTransactionCommandOutput>
{
    public async Task<CreateTransactionCommandOutput> Handle(CreateTransactionCommandInput command, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(command.AccountId, cancellationToken);
        if (account is null)
            throw new NotFoundException("Account", command.AccountId);

        Guid? resolvedCategoryId = command.CategoryId;
        string? categoryName = null;

        if (command.CategoryId.HasValue)
        {
            var category = await categoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);
            if (category is null)
                throw new NotFoundException("Category", command.CategoryId.Value);
            categoryName = category.Name;
        }
        else if (command.CategoryName is not null)
        {
            var existing = await categoryRepository.FindByNameAsync(command.CategoryName, cancellationToken);
            if (existing is not null)
            {
                resolvedCategoryId = existing.Id;
                categoryName = existing.Name;
            }
            else
            {
                CategoryGroup? group;
                if (command.CategoryGroupId.HasValue)
                {
                    group = await categoryRepository.GetGroupByIdAsync(command.CategoryGroupId.Value, cancellationToken)
                        ?? throw new NotFoundException("CategoryGroup", command.CategoryGroupId.Value);
                }
                else
                {
                    group = await categoryRepository.GetFirstUserGroupAsync(cancellationToken)
                        ?? throw new DomainException("Create a category group before adding transactions with new categories.");
                }
                var newCategory = Category.Create(group.Id, command.CategoryName);
                await categoryRepository.AddCategoryAsync(newCategory, cancellationToken);
                resolvedCategoryId = newCategory.Id;
                categoryName = newCategory.Name;
            }
        }

        var transaction = Transaction.Create(command.AccountId, command.Amount, command.Date, resolvedCategoryId, command.Memo, command.AffectsBudget);

        await transactionRepository.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTransactionCommandOutput(new TransactionDTO(
            transaction.Id,
            transaction.AccountId,
            transaction.CategoryId,
            categoryName,
            transaction.Amount,
            transaction.Date,
            transaction.Memo,
            transaction.AffectsBudget,
            transaction.CreatedAt));
    }
}
