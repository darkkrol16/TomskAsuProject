using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TomskASUProject.HallOfFame.API.Infrastructure.Contexts;
using TomskASUProject.HallOfFame.API.Models;

namespace TomskASUProject.HallOfFame.API
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider, string dataFilePath)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = serviceProvider.GetRequiredService<ILogger<SeedData>>();
            context.Database.Migrate();

            if (!context.Persons.Any())
            {
                SeedDataFromFile(dataFilePath, context);
                return;
            }

            logger.LogWarning("Database context is already set up and has demo data. Change SeedData to false in appsettings.json ({AppName})");
            return;
        }


        public static void SeedDataFromFile(string dataFilePath, ApplicationDbContext context)
        {
            IEnumerable<Person> persons = GetPersonsFromFile(dataFilePath);

            context.Persons.AddRange(persons);

            context.SaveChanges();
        }

        public static IEnumerable<Person> GetPersonsFromFile(string dataFilePath) {
            List<Person> persons = new List<Person>();

            if (File.Exists(dataFilePath))
            {
                using (StreamReader r = new StreamReader(dataFilePath))
                {
                    var json = r.ReadToEnd();
                    var jobj = JObject.Parse(json);

                    foreach (var person in jobj["persons"])
                    {
                        var name = person["name"];
                        var displayName = person["displayName"];
                        var skills = person["skills"];

                        var personEntity = new Person
                        {
                            Name = name.ToString(),
                            DisplayName = displayName.ToString(),
                        };

                        foreach (var skill in skills)
                        {
                            var skillName = skill["Name"].ToString();
                            var skillLevel = byte.Parse(skill["Level"].ToString());

                            var skillEntity = new Skill
                            {
                                Name = skillName,
                                Level = skillLevel,
                                Person = personEntity,
                            };

                            personEntity.Skills.Add(skillEntity);
                        }

                        persons.Add(personEntity);
                    }

                    return persons;
                }
            }

            throw new FileNotFoundException("File not found!");
        }
    }
}
