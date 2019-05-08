using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TomskASUProject.HallOfFame.API.Models;

namespace TomskASUProject.HallOfFame.API.DTOs
{
    public class PersonDTO
    {
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; }

        [MinLength(3)]
        [MaxLength(50)]
        public string DisplayName { get; set; }

        public List<SkillDTO> Skills { get; set; }

        public PersonDTO()
        {
            Skills = new List<SkillDTO>();

        }

        public PersonDTO(Person person)
        {
            Skills = new List<SkillDTO>();
            Name = person.Name;
            DisplayName = person.DisplayName;

            foreach (var skill in person.Skills)
            {
                Skills.Add(new SkillDTO(skill));
            }
        }
    }

    public class PersonResponseDTO : PersonDTO
    {
        public long Id { get; set; }

        public PersonResponseDTO() : base()
        {
        }

        public PersonResponseDTO(Person person) : base(person)
        {
            Id = person.Id;
        }
    }
}
