using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Domain;

namespace Config.Application.Abstractions
{
    public interface IConfigRepository
    {
        Task<List<ConfigItem>> GetActiveByAppAsync(string app, CancellationToken ct);
        Task<List<ConfigItem>> SearchAsync(string? app, string? q, CancellationToken ct);
        Task<ConfigItem?> GetAsync(string app, string name, CancellationToken ct);
        Task UpsertAsync(ConfigItem item, CancellationToken ct);
        Task DeleteAsync(string app, string name, CancellationToken ct);
    }
}
