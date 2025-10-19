using System.Collections.Immutable;
using System.Globalization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Config.Client;

public sealed class ConfigurationReader : IDisposable
{
    private readonly string _app;
    private readonly int _refreshMs;
    private readonly CancellationTokenSource _cts = new();
    private readonly IMongoCollection<Doc> _col;

    private ImmutableDictionary<string, Doc> _cache =
        ImmutableDictionary.Create<string, Doc>(StringComparer.OrdinalIgnoreCase);

    private readonly Action<string>? _log;

    public ConfigurationReader(
        string applicationName,
        string connectionString,
        int refreshTimerIntervalInMs,
        Action<string>? logger = null)
    {
        _app = applicationName;
        _refreshMs = refreshTimerIntervalInMs;
        _log = logger;

        var cli = new MongoClient(connectionString);
        var db = cli.GetDatabase("configdb");
        _col = db.GetCollection<Doc>("configs");

        try
        {
            RefreshAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _log?.Invoke($"[Refresh-INIT-ERROR] {ex}");
        }

        _ = Task.Run(RefreshLoop);
    }

    private async Task RefreshLoop()
    {
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_refreshMs));
        while (await timer.WaitForNextTickAsync(_cts.Token))
        {
            try { await RefreshAsync(); }
            catch (Exception ex) { _log?.Invoke($"[Refresh-ERROR] {ex}"); }
        }
    }

    private async Task RefreshAsync()
    {
        var list = await _col
            .Find(x => x.ApplicationName == _app && x.IsActive)
            .ToListAsync(_cts.Token);

        _log?.Invoke($"[Refresh] app='{_app}' activeCount={list.Count}");
        if (list.Count > 0)
            _log?.Invoke("[Keys] " + string.Join(", ", list.Select(z => z.Name)));

        var dict = list.ToImmutableDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        Interlocked.Exchange(ref _cache, dict);
    }



    public static object ConvertBoxed(string type, string value)
    {
        switch (type)
        {
            case "string": return value;
            case "int": return int.Parse(value, CultureInfo.InvariantCulture);
            case "double": return double.Parse(value, CultureInfo.InvariantCulture);
            case "bool": return (value == "1" || bool.Parse(value));
            default: throw new InvalidOperationException($"Unsupported type '{type}'");
        }
    }

    public T GetValue<T>(string key)
    {
        if (!_cache.TryGetValue(key, out var d))
            throw new KeyNotFoundException($"Config key not found: '{key}' for '{_app}'.");

        object boxed = ConvertBoxed(d.Type, d.Value);
        return (T)Convert.ChangeType(boxed, typeof(T), CultureInfo.InvariantCulture)!;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    private sealed class Doc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string ApplicationName { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Value { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }



}
