using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}