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

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Users()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var viewModel = new AdminEditUserViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            IsAdmin = user.IsAdmin
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(AdminEditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FindAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = model.Username;
            user.Email = model.Email;
            user.IsAdmin = model.IsAdmin;

            await _context.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        return View(model);
    }

    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Current admin cannot delete themselves
        if (user.Id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
        {
            TempData["ErrorMessage"] = "You cannot delete your own account.";
            return RedirectToAction("Users");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Users");
    }

    public async Task<IActionResult> Tasks()
    {
        var tasks = await _context.Tasks.Include(t => t.User).ToListAsync();
        return View(tasks);
    }

    public async Task<IActionResult> CreateTask()
    {
        var users = await _context.Users.ToListAsync();
        var viewModel = new AdminCreateTaskViewModel
        {
            Users = users
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(AdminCreateTaskViewModel model)
    {
        if (ModelState.IsValid)
        {
            var task = new TaskItem
            {
                UserId = model.UserId,
                TaskName = model.TaskName,
                Description = model.Description,
                TaskDate = model.TaskDate,
                Status = "Created"
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Tasks");
        }

        model.Users = await _context.Users.ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> EditTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        var users = await _context.Users.ToListAsync();

        var viewModel = new AdminEditTaskViewModel
        {
            Id = task.Id,
            UserId = task.UserId,
            TaskName = task.TaskName,
            Description = task.Description,
            TaskDate = task.TaskDate,
            Status = task.Status,
            Users = users
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditTask(AdminEditTaskViewModel model)
    {
        if (ModelState.IsValid)
        {
            var task = await _context.Tasks.FindAsync(model.Id);
            if (task == null)
            {
                return NotFound();
            }

            task.UserId = model.UserId;
            task.TaskName = model.TaskName;
            task.Description = model.Description;
            task.TaskDate = model.TaskDate;
            task.Status = model.Status;

            await _context.SaveChangesAsync();
            return RedirectToAction("Tasks");
        }

        model.Users = await _context.Users.ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return RedirectToAction("Tasks");
    }
}