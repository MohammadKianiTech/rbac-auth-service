using Asp.Versioning;
using Evalify.Application.Evaluations.Commands.Create;
using Evalify.Application.Evaluations.Commands.Delete;
using Evalify.Application.Evaluations.Commands.Publish;
using Evalify.Application.Evaluations.Commands.Update;
using Evalify.Application.Evaluations.Queries.GetEvaluations;
using Evalify.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evalify.Api.Controllers.Evaluations;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/evaluations")]
public class EvaluationsController(ISender _sender) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.EvaluationsRead)]
    public async Task<IActionResult> GetEvaluations(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? sortColumn = null,
        string? sortOrder = "desc",
        CancellationToken cancellationToken = default)
    {
        var query = new GetEvaluationsQuery(page, pageSize, searchTerm, sortColumn, sortOrder);

        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }

    [HttpPost]
    [HasPermission(Permissions.EvaluationsCreate)]
    public async Task<IActionResult> Add([FromBody] CreateEvaluationRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateEvaluationCommand(request.Title, request.Description, request.StartDate, request.EndDate);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? StatusCode(StatusCodes.Status201Created, new { id = result.Value }) : NotFound();
    }

    [HttpPut]
    [HasPermission(Permissions.EvaluationsUpdate)]
    public async Task<IActionResult> Update([FromBody] UpdateEvaluationRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateEvaluationCommand(request.Id, request.Title, request.Description, request.StartDate, request.EndDate);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound();
    }
    [HttpPut("{id}/publish")]
    [HasPermission(Permissions.EvaluationsUpdate)]
    public async Task<IActionResult> Publish([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new PublishEvaluationCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.EvaluationsDelete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteEvaluationCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound();
    }
}