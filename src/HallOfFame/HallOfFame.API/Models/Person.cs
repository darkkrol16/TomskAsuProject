using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TomskASUProject.HallOfFame.API.Models
{
    public class Person
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }

        public List<Skill> Skills { get; set; }

        public Person()
        {
            Skills = new List<Skill>();
        }
    }
}
