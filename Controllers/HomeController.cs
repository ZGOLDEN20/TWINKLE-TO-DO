using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TWINKLE_TO_DO.Models;
using TWINKLE_TO_DO.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace TWINKLE_TO_DO.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Dashboard");
        }
        return View("IndexT");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Dashboard");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email already in use");
                return View(model);
            }

            var hashedPassword = HashPassword(model.Password);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = hashedPassword
            };

            var userSettings = new UserSettings
            {
                Id = user.Id,
                Theme = "system",
                NotificationsEnabled = true,
                User = user
            };

            user.UserSettings = userSettings;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SignInUserAsync(user);

            return RedirectToAction("Dashboard");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Dashboard");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var hashedPassword = HashPassword(model.Password);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null && user.Password == hashedPassword)
            {
                await SignInUserAsync(user);


                if (user.IsAdmin)
                {
                    return RedirectToAction("AdminDashboard");
                }
                else
                {
                    return RedirectToAction("Dashboard");
                }
            }

            ModelState.AddModelError("", "Invalid email or password");
        }

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _context.Users
            .Include(u => u.Tasks)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Logout");
        }

        var viewModel = new DashboardViewModel
        {
            Username = user.Username,
            Tasks = user.Tasks?.Where(t => t.Status != "Completed").ToList() ?? new List<TaskItem>()
        };

        return View(viewModel);
    }

    [Authorize]
    public IActionResult CreateTask()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskViewModel model)
    {
        if (ModelState.IsValid)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var task = new TaskItem
            {
                UserId = userId,
                TaskName = model.TaskName,
                Description = model.Description,
                TaskDate = model.TaskDate,
                Status = "Created"
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _context.Users
            .Include(u => u.UserSettings)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Logout");
        }

        var viewModel = new ProfileViewModel
        {
            Username = user.Username,
            Email = user.Email,
            Theme = user.UserSettings?.Theme ?? "system",
            NotificationsEnabled = user.UserSettings?.NotificationsEnabled ?? true
        };

        return View(viewModel);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users
                .Include(u => u.UserSettings)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Logout");
            }

            user.Username = model.Username;

            if (user.UserSettings == null)
            {
                user.UserSettings = new UserSettings
                {
                    Id = user.Id,
                    Theme = model.Theme,
                    NotificationsEnabled = model.NotificationsEnabled
                };
            }
            else
            {
                user.UserSettings.Theme = model.Theme;
                user.UserSettings.NotificationsEnabled = model.NotificationsEnabled;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Profile");
        }

        return View("Profile", model);
    }

    [Authorize]
    public async Task<IActionResult> Task()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var tasks = await _context.Tasks
            .Where(t => t.UserId == userId)
            .ToListAsync();

        return View(tasks);
    }

    [Authorize]
    public async Task<IActionResult> CompleteTask(int id)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task != null)
        {
            task.Status = "Completed";
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Dashboard");
    }

    [Authorize]
    public async Task<IActionResult> DeleteTask(int id)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Dashboard");
    }

    [Authorize]
    public async Task<IActionResult> EditTask(int id)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditTask(int id, TaskItem model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (model.UserId != userId)
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Dashboard");
        }
        return View(model);
    }

    private bool TaskExists(int id)
    {
        return _context.Tasks.Any(e => e.Id == id);
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

    private async Task SignInUserAsync(User user)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
    };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminDashboard()
    {
        var users = await _context.Users.ToListAsync();
        var tasks = await _context.Tasks.Include(t => t.User).ToListAsync();

        var viewModel = new AdminDashboardViewModel
        {
            Users = users,
            Tasks = tasks
        };

        return View(viewModel);
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}