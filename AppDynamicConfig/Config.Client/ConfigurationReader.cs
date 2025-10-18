using System.Collections.Immutable;
using System.Globalization;
using MongoDB.Driver;

namespace Config.Client;

public sealed class ConfigurationReader : IDisposable
{
    private readonly string _app;
    private readonly int _refreshMs;
    private readonly CancellationTokenSource _cts = new();
    private readonly IMongoCollection<Doc> _col;
    private ImmutableDictionary<string, Doc> _cache = ImmutableDictionary<string, Doc>.Empty;

    public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
    {
        _app = applicationName;
        _refreshMs = refreshTimerIntervalInMs;

        var cli = new MongoClient(connectionString);
        var db = cli.GetDatabase("configdb");
        _col = db.GetCollection<Doc>("configs");

        Task.Run(RefreshAsync);
        Task.Run(RefreshLoop);
    }

    private async Task RefreshLoop()
    {
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_refreshMs));
        while (await timer.WaitForNextTickAsync(_cts.Token))
        {
            try { await RefreshAsync(); } catch { /* last good snapshot */ }
        }
    }

    private async Task RefreshAsync()
    {
        var list = await _col.Find(x => x.ApplicationName == _app && x.IsActive)
                             .ToListAsync(_cts.Token);
        var dict = list.ToImmutableDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        Interlocked.Exchange(ref _cache, dict);
    }

    public T GetValue<T>(string key)
    {
        if (!_cache.TryGetValue(key, out var d))
            throw new KeyNotFoundException($"Config key not found: '{key}' for '{_app}'.");

        object boxed = d.Type switch
        {
            "string" => d.Value,
            "int" => int.Parse(d.Value, CultureInfo.InvariantCulture),
            "double" => double.Parse(d.Value, CultureInfo.InvariantCulture),
            "bool" => (d.Value == "1" || bool.Parse(d.Value)),
            _ => throw new InvalidOperationException($"Unsupported type '{d.Type}'")
        };
        return (T)Convert.ChangeType(boxed, typeof(T), CultureInfo.InvariantCulture)!;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    private sealed class Doc
    {
        public string Id { get; set; } = default!;
        public string ApplicationName { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Value { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
