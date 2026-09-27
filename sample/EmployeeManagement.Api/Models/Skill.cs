using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Models;

public class Skill
{
    public int SkillId { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    public ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
}
