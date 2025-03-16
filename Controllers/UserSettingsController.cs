// This file is not being used anywhere in this project
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TWINKLE_TO_DO.Data;
using TWINKLE_TO_DO.Models;

[Route("api/[controller]")]
[ApiController]
public class UserSettingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserSettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/usersettings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserSettings>>> GetUserSettings()
    {
        return await _context.UserSettings.ToListAsync();
    }

    // GET: api/usersettings/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserSettings>> GetUserSetting(int id)
    {
        var setting = await _context.UserSettings.FindAsync(id);
        if (setting == null) return NotFound();
        return setting;
    }

    // POST: api/usersettings
    [HttpPost]
    public async Task<ActionResult<UserSettings>> PostUserSetting(UserSettings setting)
    {
        _context.UserSettings.Add(setting);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserSetting), new { id = setting.Id }, setting);
    }

    // PUT: api/usersettings/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUserSetting(int id, UserSettings setting)
    {
        if (id != setting.Id) return BadRequest();
        _context.Entry(setting).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/usersettings/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserSetting(int id)
    {
        var setting = await _context.UserSettings.FindAsync(id);
        if (setting == null) return NotFound();
        _context.UserSettings.Remove(setting);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
