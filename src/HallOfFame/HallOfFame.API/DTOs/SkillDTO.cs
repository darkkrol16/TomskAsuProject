using System.ComponentModel.DataAnnotations;
using TomskASUProject.HallOfFame.API.Models;

namespace TomskASUProject.HallOfFame.API.DTOs
{
    public class SkillDTO
    {
		[Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }
		
		[Required]
        [Range(0, 10)]
        public byte Level { get; set; }

        public SkillDTO() { }

        public SkillDTO(Skill skill)
        {
            Name = skill.Name;
            Level = skill.Level;
        }
    }
}