using Admin.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

public sealed class ConfigsController : Controller
{
    private readonly ConfigApiClient _api;
    private const string DefaultApp = "SERVICE-A";

    public ConfigsController(ConfigApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? app = null, string? q = null)
    {
        var data = await _api.GetListAsync(app, q);
        ViewBag.AppName = string.IsNullOrWhiteSpace(app) ? "All" : app;
        ViewBag.App = app ?? "";
        ViewBag.Query = q ?? "";
        return View(data);
    }

    public IActionResult Create(string? app)
        => View(new ConfigApiClient.UpsertDto(app ?? DefaultApp, "", "string", "", true));

    [HttpPost]
    public async Task<IActionResult> Create(ConfigApiClient.UpsertDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var ok = await _api.UpsertAsync(dto);
        if (!ok) { ModelState.AddModelError("", "Create failed"); return View(dto); }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string app, string name)
    {
        var list = await _api.ListAsync(app) ?? new();
        var item = list.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (item is null) return NotFound();
        var dto = new ConfigApiClient.UpsertDto(app, name, item.Type, item.Value, item.IsActive);
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ConfigApiClient.UpsertDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var ok = await _api.UpsertAsync(dto);
        if (!ok) { ModelState.AddModelError("", "Update failed"); return View(dto); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string app, string name)
    {
        await _api.DeleteAsync(app, name);
        return RedirectToAction(nameof(Index));
    }
}
