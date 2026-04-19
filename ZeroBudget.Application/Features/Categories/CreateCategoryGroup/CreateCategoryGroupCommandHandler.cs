using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Application.Features.Categories.CreateCategoryGroup;

public class CreateCategoryGroupCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryGroupCommandInput, CreateCategoryGroupCommandOutput>
{
    public async Task<CreateCategoryGroupCommandOutput> Handle(
        CreateCategoryGroupCommandInput command,
        CancellationToken cancellationToken)
    {
        var group = CategoryGroup.Create(command.Name);

        await categoryRepository.AddGroupAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCategoryGroupCommandOutput(
            new CategoryGroupDTO(group.Id, group.Name, group.SortOrder, []));
    }
}
