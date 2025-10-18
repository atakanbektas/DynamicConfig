using Config.Application.Abstractions;
using MediatR;

namespace Config.Application.Commands;

public sealed record DeleteConfigCommand(string App, string Name) : IRequest<Unit>;

public sealed class DeleteConfigHandler : IRequestHandler<DeleteConfigCommand, Unit>
{
    private readonly IConfigRepository _repo;
    public DeleteConfigHandler(IConfigRepository repo) => _repo = repo;

    public async Task<Unit> Handle(DeleteConfigCommand c, CancellationToken ct)
    {
        await _repo.DeleteAsync(c.App, c.Name, ct);
        return Unit.Value;
    }
}
