using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTransactionCommandInput, UpdateTransactionCommandOutput>
{
    public async Task<UpdateTransactionCommandOutput> Handle(UpdateTransactionCommandInput command, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(command.TransactionId, cancellationToken);
        if (transaction is null)
            throw new NotFoundException("Transaction", command.TransactionId);

        string? categoryName = null;
        if (command.CategoryId.HasValue)
        {
            var category = await categoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);
            if (category is null)
                throw new NotFoundException("Category", command.CategoryId.Value);
            categoryName = category.Name;
        }

        transaction.Update(command.Amount, command.CategoryId, command.Date, command.Memo, command.AffectsBudget);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateTransactionCommandOutput(new TransactionDTO(
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
