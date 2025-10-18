using Config.Application.Abstractions;
using Config.Domain;
using MediatR;

namespace Config.Application.Queries;

public sealed record ListByAppQuery(string App) : IRequest<List<ConfigItem>>;

public sealed class ListByAppHandler : IRequestHandler<ListByAppQuery, List<ConfigItem>>
{
    private readonly IConfigRepository _repo;
    public ListByAppHandler(IConfigRepository repo) => _repo = repo;
    public Task<List<ConfigItem>> Handle(ListByAppQuery q, CancellationToken ct)
        => _repo.GetActiveByAppAsync(q.App, ct);
}
