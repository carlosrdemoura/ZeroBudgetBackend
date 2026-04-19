using MediatR;
using ZeroBudget.Application.Common;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;

namespace ZeroBudget.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryHandler(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository) : IRequestHandler<GetTransactionsQueryInput, GetTransactionsQueryOutput>
{
    public async Task<GetTransactionsQueryOutput> Handle(GetTransactionsQueryInput query, CancellationToken cancellationToken)
    {
        var paged = await transactionRepository.GetPagedAsync(
            query.AccountId,
            query.FromDate,
            query.ToDate,
            query.Search,
            query.Page,
            query.PageSize,
            cancellationToken);

        var categoryIds = paged.Items
            .Where(t => t.CategoryId.HasValue)
            .Select(t => t.CategoryId!.Value)
            .Distinct()
            .ToHashSet();

        Dictionary<Guid, string> categoryNames = [];
        if (categoryIds.Count > 0)
        {
            var categories = await categoryRepository.GetCategoriesByIdsAsync(categoryIds, cancellationToken);
            categoryNames = categories.ToDictionary(c => c.Id, c => c.Name);
        }

        var items = paged.Items.Select(t => new TransactionDTO(
            t.Id,
            t.AccountId,
            t.CategoryId,
            t.CategoryId.HasValue ? categoryNames.GetValueOrDefault(t.CategoryId.Value) : null,
            t.Amount,
            t.Date,
            t.Memo,
            t.AffectsBudget,
            t.CreatedAt)).ToList();

        return new GetTransactionsQueryOutput(new PagedResult<TransactionDTO>(items, paged.TotalCount, paged.Page, paged.PageSize));
    }
}
