using Dapper;
using Evalify.Application.Abstractions.Data;
using Evalify.Application.Abstractions.Messaging;
using Evalify.Application.Common.Pagination;
using Evalify.Domain.Abstractions;

namespace Evalify.Application.Evaluations.Queries.GetEvaluations;

internal sealed class GetEvaluationsQueryHandler(
    ISqlConnectionFactory _sqlConnectionFactory) : IQueryHandler<GetEvaluationsQuery, PagedList<EvaluationResponse>>
{
    public async Task<Result<PagedList<EvaluationResponse>>> Handle(GetEvaluationsQuery request, CancellationToken cancellationToken)
    {
        string sortColumn = request.SortColumn?.ToLower() switch
        {
            "title" => "title",
            "createdat" => "created_at",
            "description" => "description",
            _ => "created_at"
        };
        string sortOrder = request.SortOrder?.ToLower() == "asc" ? "ASC" : "DESC";
        using var connection = _sqlConnectionFactory.CreateConnection();
        const string baseSql = """
            FROM evaluations
            WHERE 
            (
                @search IS NULL
                OR (
                    LOWER(title) LIKE '%' || LOWER(@search) || '%'
                    OR LOWER(description) LIKE '%' || LOWER(@search) || '%'
                )
            )
        """;
        string countSql = $"SELECT COUNT(*) {baseSql};";
        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { search = request.SearchTerm });

        string dataSql = $"""
            SELECT 
                id AS Id,
                title AS Title,
                description AS Description,
                CASE status
                    WHEN 0 THEN 'Draft'
                    WHEN 1 THEN 'Active'
                    WHEN 2 THEN 'Closed'
                    ELSE 'Unknown'
                END AS Status,
                start_date AS StartDate,
                end_date AS EndDate,
                created_at AS CreatedAt
            {baseSql}
            ORDER BY {sortColumn} {sortOrder}
            LIMIT @limit
            OFFSET @offset;
        """;
        int page = request.Page < 1 ? 1 : request.Page;
        int pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        int offset = (page - 1) * pageSize;
        var items = await connection.QueryAsync<EvaluationResponse>(
            dataSql,
            new
            {
                search = request.SearchTerm,
                limit = pageSize,
                offset
            }
        );
        var pagedData = new PagedList<EvaluationResponse>(items.ToList(), page, pageSize, totalCount);
        return Result.Success(pagedData);
    }
}