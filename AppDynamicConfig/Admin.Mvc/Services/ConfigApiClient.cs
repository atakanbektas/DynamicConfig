// Admin.Mvc/Services/ConfigApiClient.cs
using System.Net.Http.Json;

namespace Admin.Mvc.Services;

public sealed class ConfigApiClient
{
    private readonly HttpClient _http;
    public ConfigApiClient(HttpClient http) => _http = http;

    // ✅ Service (ApplicationName) ve IsActive eklendi
    public record ConfigRow(
        string ApplicationName,
        string Name,
        string Type,
        string Value,
        bool IsActive,
        DateTime UpdatedAtUtc
    );

    public record UpsertDto(string ApplicationName, string Name, string Type, string Value, bool IsActive);

    public async Task<List<ConfigRow>> GetListAsync(string? app, string? q)
    {
        var url = "/api/configs";
        var qs = new List<string>();
        if (!string.IsNullOrWhiteSpace(app)) qs.Add($"app={Uri.EscapeDataString(app)}");
        if (!string.IsNullOrWhiteSpace(q)) qs.Add($"q={Uri.EscapeDataString(q)}");
        if (qs.Count > 0) url += "?" + string.Join("&", qs);

        var res = await _http.GetFromJsonAsync<List<ConfigRow>>(url);
        return res ?? new();
    }

    public Task<List<ConfigRow>?> ListAsync(string app)
        => _http.GetFromJsonAsync<List<ConfigRow>>($"/api/configs?appName={Uri.EscapeDataString(app)}");

    public async Task<bool> UpsertAsync(UpsertDto dto)
    {
        var res = await _http.PostAsJsonAsync("/api/configs", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string app, string name)
    {
        var res = await _http.DeleteAsync($"/api/configs/{Uri.EscapeDataString(app)}/{Uri.EscapeDataString(name)}");
        return res.IsSuccessStatusCode;
    }
}
