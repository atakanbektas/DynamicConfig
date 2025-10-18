using Config.Application.Abstractions;
using Config.Domain;
using MongoDB.Driver;

namespace Config.Persistence;

public sealed class MongoConfigRepository : IConfigRepository
{
    private readonly IMongoCollection<ConfigItem> _col;

    public MongoConfigRepository(string conn)
    {
        var cli = new MongoClient(conn);
        var db = cli.GetDatabase("configdb");
        _col = db.GetCollection<ConfigItem>("configs");

        _col.Indexes.CreateOne(new CreateIndexModel<ConfigItem>(
            Builders<ConfigItem>.IndexKeys
                .Ascending(x => x.ApplicationName)
                .Ascending(x => x.Name),
            new CreateIndexOptions { Unique = true }));
    }

    public Task<List<ConfigItem>> SearchAsync(string? app, string? q, CancellationToken ct)
    {
        var filter =
            Builders<ConfigItem>.Filter.Where(x =>
                (string.IsNullOrWhiteSpace(app) || x.ApplicationName == app) &&
                (string.IsNullOrWhiteSpace(q) || x.Name.ToLower().Contains(q.ToLower()))
            );

        return _col.Find(filter)
                   .SortBy(x => x.ApplicationName)
                   .ThenBy(x => x.Name)
                   .ToListAsync(ct);
    }


    public Task<List<ConfigItem>> GetActiveByAppAsync(string app, CancellationToken ct)
        => _col.Find(x => x.ApplicationName == app && x.IsActive).ToListAsync(ct);

    public Task<ConfigItem?> GetAsync(string app, string name, CancellationToken ct)
        => _col.Find(x => x.ApplicationName == app && x.Name == name).FirstOrDefaultAsync(ct);

    public Task UpsertAsync(ConfigItem item, CancellationToken ct)
    {
        var filter = Builders<ConfigItem>.Filter.Eq(x => x.ApplicationName, item.ApplicationName) &
                     Builders<ConfigItem>.Filter.Eq(x => x.Name, item.Name);

        var update = Builders<ConfigItem>.Update
            .Set(x => x.Type, item.Type)
            .Set(x => x.Value, item.Value)
            .Set(x => x.IsActive, item.IsActive)
            .Set(x => x.UpdatedAtUtc, DateTime.UtcNow);

        return _col.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, ct);
    }

    public Task DeleteAsync(string app, string name, CancellationToken ct)
        => _col.DeleteOneAsync(x => x.ApplicationName == app && x.Name == name, ct);
}
