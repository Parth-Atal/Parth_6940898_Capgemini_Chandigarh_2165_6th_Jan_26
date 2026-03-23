using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EventRegistrationSystem.Models;

public class IndexModel : PageModel
{
    public List<EventRegistration> Participants { get; set; }

    public void OnGet()
    {
        Participants = RegisterModel.registrations;
    }

    public IActionResult OnPostDelete(int id)
    {
        var item = RegisterModel.registrations.FirstOrDefault(x => x.Id == id);

        if (item != null)
            RegisterModel.registrations.Remove(item);

        return RedirectToPage();
    }
}