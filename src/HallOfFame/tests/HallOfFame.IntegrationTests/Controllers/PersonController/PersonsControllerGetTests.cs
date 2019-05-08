using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TomskASUProject.HallOfFame.API;
using TomskASUProject.HallOfFame.API.DTOs;
using Xunit;

namespace TomskAsuProject.HallOfFame.IntegrationTests.Controllers
{
    public class PersonsControllerGetTests : PersonsControllerTestBase
    {
        public PersonsControllerGetTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetPersons_Returns_Persons()
        {
            //Arrange
            var httpClient = _factory.CreateClient();
            var personsFromFileIEnumerable = SeedData.GetPersonsFromFile(initDataFilePath);
            var personsFromFile = personsFromFileIEnumerable.ToList();


            //Act
            var httpResponse = await httpClient.GetAsync("/api/v1/persons");


            //Assert
            httpResponse.EnsureSuccessStatusCode();

            var personsIEnumerable = JsonConvert.DeserializeObject<IEnumerable<PersonResponseDTO>>(await httpResponse.Content.ReadAsStringAsync());
            var personsResult = personsIEnumerable.ToList();
            Assert.NotEmpty(personsResult);

            for (int i = 0; i < personsFromFile.Count; i++)
            {
                Assert.Equal(personsFromFile[i].Name, personsResult[i].Name);
                Assert.Equal(personsFromFile[i].DisplayName, personsResult[i].DisplayName);
            }
        }

        [Fact]
        public async Task GetPerson_With_Invalid_Id_Returns_NotFound()
        {
            //Arrange
            var httpClient = _factory.CreateClient();

            //Act
            var httpResponse = await httpClient.GetAsync("/api/v1/person/0");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact]
        public async Task GetPerson_With_Valid_Id_Returns_Valid_User()
        {
            //Arrange
            var httpClient = _factory.CreateClient();
            var personExpected = GetPersonFirst();

            //Act
            var httpResponse = await httpClient.GetAsync("/api/v1/person/1");


            //Assert
            httpResponse.EnsureSuccessStatusCode();

            var personResult = JsonConvert.DeserializeObject<PersonResponseDTO>(await httpResponse.Content.ReadAsStringAsync());
            AssertEqualPersons(personExpected, personResult);
        }

    }
}
