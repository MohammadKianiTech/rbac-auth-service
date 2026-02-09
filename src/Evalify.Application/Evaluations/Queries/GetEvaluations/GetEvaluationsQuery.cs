using Evalify.Application.Abstractions.Messaging;
using Evalify.Application.Common.Pagination;

namespace Evalify.Application.Evaluations.Queries.GetEvaluations;

public sealed record GetEvaluationsQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    string? SortColumn,
    string? SortOrder) : IQuery<PagedList<EvaluationResponse>>;