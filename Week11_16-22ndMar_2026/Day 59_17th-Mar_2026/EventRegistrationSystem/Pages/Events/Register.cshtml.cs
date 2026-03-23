using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EventRegistrationSystem.Models;

public class RegisterModel : PageModel
{
    public static List<EventRegistration> registrations = new List<EventRegistration>();

    [BindProperty]
    public EventRegistration Registration { get; set; }

    public void OnGet()
    {
        Registration = new EventRegistration();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        Registration.Id = registrations.Count > 0
            ? registrations.Max(r => r.Id) + 1
            : 1;
        registrations.Add(Registration);

        return RedirectToPage("./Index");
    }
}