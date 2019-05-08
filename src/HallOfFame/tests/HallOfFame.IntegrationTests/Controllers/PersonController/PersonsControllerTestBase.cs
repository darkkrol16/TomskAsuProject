using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TomskASUProject.HallOfFame.API;
using TomskASUProject.HallOfFame.API.DTOs;
using TomskASUProject.HallOfFame.API.Models;
using Xunit;

namespace TomskAsuProject.HallOfFame.IntegrationTests.Controllers
{
    public class PersonsControllerTestBase : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly CustomWebApplicationFactory<Startup> _factory;
        protected readonly IEnumerable<Person> personsFromFile = SeedData.GetPersonsFromFile(initDataFilePath);

        protected const string initDataFilePath = "initdata.json";

        public PersonsControllerTestBase(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;

            _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
                });
            });
        }

        protected void AssertEqualPersons(PersonDTO personExpected, PersonResponseDTO personResult)
        {
            Assert.Equal(personExpected.Name, personResult.Name);
            Assert.Equal(personExpected.DisplayName, personResult.DisplayName);
            Assert.NotEmpty(personResult.Skills);
            for (int i = 0; i < personExpected.Skills.Count; i++)
            {
                Assert.Equal(personExpected.Skills[i].Name, personResult.Skills[i].Name);
                Assert.Equal(personExpected.Skills[i].Level, personResult.Skills[i].Level);
            }
        }

        protected void AssertModelStateInvalid(JObject modelState)
        {
            Assert.True(modelState.ContainsKey("Name"));
            Assert.True(modelState.ContainsKey("DisplayName"));
            Assert.True(modelState.ContainsKey("Skills[0].Level"));
        }

        protected PersonDTO GetPersonFirst()
        {
            return
                new PersonDTO(personsFromFile.First());
        }

        protected PersonDTO GetPersonInvalid()
        {
            return new PersonDTO
            {
                Name = "s", //Min length is 3
                DisplayName = "t", //The same
                Skills = new List<SkillDTO>
                {
                    new SkillDTO
                    {
                        Name = "test",
                        Level = 200, //Range(0, 10)
                    }
                }
            };
        }
    }
}
