using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Categories.CreateCategory;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommandInput, CreateCategoryCommandOutput>
{
    public async Task<CreateCategoryCommandOutput> Handle(
        CreateCategoryCommandInput command,
        CancellationToken cancellationToken)
    {
        var group = await categoryRepository.GetGroupByIdAsync(command.GroupId, cancellationToken);
        if (group is null)
            throw new NotFoundException("CategoryGroup", command.GroupId);

        var category = Category.Create(command.GroupId, command.Name);

        await categoryRepository.AddCategoryAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCategoryCommandOutput(
            new CategoryDTO(category.Id, category.GroupId, category.Name, category.SortOrder));
    }
}
