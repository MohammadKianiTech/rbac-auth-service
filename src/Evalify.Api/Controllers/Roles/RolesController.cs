using Asp.Versioning;
using Evalify.Application.Roles.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evalify.Api.Controllers.Roles;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/roles")]
public class RolesController(ISender _sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLoggedInUser(CancellationToken cancellationToken)
    {
        var query = new GetRolesQuery();

        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }
}
