using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Categories.UpdateCategory;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommandInput, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryCommandInput command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException("Category", command.CategoryId);

        category.Rename(command.Name);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
