using DynamicConfig.Models;
using DynamicConfig.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;


public class IndexModel : PageModel
{
    private readonly IConfigService _svc;
    public IndexModel(IConfigService svc) => _svc = svc;

    [BindProperty(Name = "app", SupportsGet = true)]
    public string App { get; set; }

    public List<Config> Items { get; private set; } = new List<Config>();

    public IActionResult OnGet(string? app)
    {
        Items = _svc.GetAll(App).ToList();
        return Page();
    }

    // ---- Create (parametre ile bağla) ----
    public IActionResult OnPostCreate([FromForm] CreateInput createForm, [FromForm] string? app)
    {
        if (!ModelState.IsValid)
        {
            Items = _svc.GetAll(App).ToList();
            return RedirectToPage();
        }

        var dto = new ConfigDto
        {
            Name = createForm.Name.Trim(),
            Type = createForm.Type,
            Value = createForm.Value.Trim(),
            IsActive = createForm.IsActive
        };

        _svc.Add(dto, App);
        return RedirectToPage();
    }

    // ---- Update (parametre ile bağla) ----
    public IActionResult OnPostUpdate([FromForm] UpdateInput updateForm, [FromForm] string? app)
    {

        var dto = new ConfigDto
        {
            Type = updateForm.Type,
            Value = updateForm.Value.Trim(),
            IsActive = updateForm.IsActive,
            Name = updateForm.Name
        };

        var affected = _svc.Update(updateForm.Id, dto, App);
        if (affected.Id == 0) return NotFound();

        return RedirectToPage();
    }

    // ---- Delete (parametre ile bağla) ----
    public IActionResult OnPostDelete([FromForm] int id, [FromForm] string? app)
    {
        var affected = _svc.Delete(id, App);
        if (affected == 0) return NotFound();
        return RedirectToPage();
    }
}

public class CreateInput
{
    [Required, MaxLength(256)] public string Name { get; set; } = default!;
    [Required] public string Type { get; set; } = "string";
    public string Value { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateInput
{
    [Required] public int Id { get; set; }
    [Required, MaxLength(256)] public string Name { get; set; } = default!;
    [Required] public string Type { get; set; } = "string";
    public string Value { get; set; }
    public bool IsActive { get; set; }
}
