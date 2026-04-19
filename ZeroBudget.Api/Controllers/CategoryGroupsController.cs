using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZeroBudget.Application.Features.Categories.CreateCategory;
using ZeroBudget.Application.Features.Categories.CreateCategoryGroup;
using ZeroBudget.Application.Features.Categories.DeleteCategory;
using ZeroBudget.Application.Features.Categories.DeleteCategoryGroup;
using ZeroBudget.Application.Features.Categories.GetCategoryGroups;
using ZeroBudget.Application.Features.Categories.MoveCategory;
using ZeroBudget.Application.Features.Categories.ReorderCategories;
using ZeroBudget.Application.Features.Categories.ReorderCategoryGroups;
using ZeroBudget.Application.Features.Categories.UpdateCategory;
using ZeroBudget.Application.Features.Categories.UpdateCategoryGroup;

namespace ZeroBudget.Api.Controllers;

[Route("api/category-groups")]
public class CategoryGroupsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetCategoryGroupsQueryOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCategoryGroupsQueryInput(), ct);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateCategoryGroupCommandOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryGroupRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateCategoryGroupCommandInput(request.Name), ct);
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpPut("{groupId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid groupId,
        [FromBody] UpdateCategoryGroupRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new UpdateCategoryGroupCommandInput(groupId, request.Name), ct);
        return NoContent();
    }

    [HttpDelete("{groupId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid groupId, [FromBody] DeleteCategoryGroupRequest? request, CancellationToken ct)
    {
        await mediator.Send(new DeleteCategoryGroupCommandInput(groupId, request?.TargetCategoryId), ct);
        return NoContent();
    }

    [HttpPatch("reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderGroups([FromBody] List<GroupSortItemRequest> items, CancellationToken ct)
    {
        await mediator.Send(new ReorderCategoryGroupsCommandInput(
            items.Select(i => new GroupSortItem(i.GroupId, i.SortOrder))), ct);
        return NoContent();
    }

    [HttpPost("{groupId:guid}/categories")]
    [ProducesResponseType(typeof(CreateCategoryCommandOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateCategory(
        Guid groupId,
        [FromBody] CreateCategoryRequest request,
        CancellationToken ct)
    {
        var result = await mediator.Send(new CreateCategoryCommandInput(groupId, request.Name), ct);
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpPut("{groupId:guid}/categories/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(
        Guid groupId,
        Guid categoryId,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new UpdateCategoryCommandInput(categoryId, request.Name), ct);
        return NoContent();
    }

    [HttpDelete("{groupId:guid}/categories/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(
        Guid groupId,
        Guid categoryId,
        [FromBody] DeleteCategoryRequest? request,
        CancellationToken ct)
    {
        await mediator.Send(new DeleteCategoryCommandInput(categoryId, request?.TargetCategoryId), ct);
        return NoContent();
    }

    [HttpPatch("{groupId:guid}/categories/reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderCategories(
        Guid groupId,
        [FromBody] List<CategorySortItemRequest> items,
        CancellationToken ct)
    {
        await mediator.Send(new ReorderCategoriesCommandInput(
            groupId,
            items.Select(i => new CategorySortItem(i.CategoryId, i.SortOrder))), ct);
        return NoContent();
    }

    [HttpPatch("{groupId:guid}/categories/{categoryId:guid}/move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveCategory(
        Guid groupId,
        Guid categoryId,
        [FromBody] MoveCategoryRequest request,
        CancellationToken ct)
    {
        await mediator.Send(new MoveCategoryCommandInput(categoryId, request.TargetGroupId), ct);
        return NoContent();
    }
}

public class CreateCategoryGroupRequest
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateCategoryGroupRequest
{
    public string Name { get; set; } = string.Empty;
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
}

public class DeleteCategoryGroupRequest
{
    public Guid? TargetCategoryId { get; set; }
}

public class DeleteCategoryRequest
{
    public Guid? TargetCategoryId { get; set; }
}

public class GroupSortItemRequest
{
    public Guid GroupId { get; set; }
    public int SortOrder { get; set; }
}

public class CategorySortItemRequest
{
    public Guid CategoryId { get; set; }
    public int SortOrder { get; set; }
}

public class MoveCategoryRequest
{
    public Guid TargetGroupId { get; set; }
}
