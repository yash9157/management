using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/skills")]
public class SkillsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [HttpGet("lookup")]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetSkills() => Ok(await dbContext.Skills.AsNoTracking().OrderBy(item => item.Name).Select(item => new LookupDto(item.SkillId, item.Name)).ToListAsync());

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Skill>> Create(NamedResourceRequest request)
    {
        var name = request.Name.Trim();
        if (await dbContext.Skills.AnyAsync(item => item.Name == name))
            return Conflict(new { message = "A skill with this name already exists." });
        var skill = new Skill { Name = name };
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSkills), new { id = skill.SkillId }, skill);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Skill>> Update(int id, NamedResourceRequest request)
    {
        var skill = await dbContext.Skills.FindAsync(id);
        if (skill is null) return NotFound();
        var name = request.Name.Trim();
        if (await dbContext.Skills.AnyAsync(item => item.Name == name && item.SkillId != id))
            return Conflict(new { message = "A skill with this name already exists." });
        skill.Name = name;
        await dbContext.SaveChangesAsync();
        return Ok(skill);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var skill = await dbContext.Skills.Include(item => item.EmployeeSkills).SingleOrDefaultAsync(item => item.SkillId == id);
        if (skill is null) return NotFound();
        if (skill.EmployeeSkills.Count > 0) return Conflict(new { message = "Remove this skill from employees first." });
        dbContext.Skills.Remove(skill);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
