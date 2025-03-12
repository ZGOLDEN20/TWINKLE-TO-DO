using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TWINKLE_TO_DO.Models;

namespace TWINKLE_TO_DO.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult CreateTask()
    {
        return View();
    }
    public IActionResult Dashboard()
    {
        return View();
    }
    public IActionResult IndexT()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }
    public IActionResult Profile()
    {
        return View();
    }
    public IActionResult Register()
    {
        return View();
    }
    public IActionResult Task()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }
   
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
