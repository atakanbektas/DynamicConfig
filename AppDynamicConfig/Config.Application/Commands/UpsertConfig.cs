using Config.Application.Abstractions;
using Config.Application.DTOs;
using Config.Domain;
using MediatR;

namespace Config.Application.Commands;

public sealed record UpsertConfigCommand(ConfigDto Dto) : IRequest<Unit>;

public sealed class UpsertConfigHandler : IRequestHandler<UpsertConfigCommand, Unit>
{
    private readonly IConfigRepository _repo;
    public UpsertConfigHandler(IConfigRepository repo) => _repo = repo;

    public async Task<Unit> Handle(UpsertConfigCommand c, CancellationToken ct)
    {
        var d = c.Dto;
        var item = new ConfigItem
        {
            ApplicationName = d.ApplicationName,
            Name = d.Name,
            Type = d.Type,
            Value = d.Value,
            IsActive = d.IsActive,
            UpdatedAtUtc = DateTime.UtcNow
        };
        await _repo.UpsertAsync(item, ct);
        return Unit.Value;
    }
}
