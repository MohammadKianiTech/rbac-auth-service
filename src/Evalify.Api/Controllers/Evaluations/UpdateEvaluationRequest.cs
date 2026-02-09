using System.Text.Json.Serialization;

namespace Evalify.Api.Controllers.Evaluations;

public record UpdateEvaluationRequest(
     [property: JsonRequired] Guid Id,
    string Title,
    string Description,
    DateTime? StartDate,
    DateTime? EndDate);