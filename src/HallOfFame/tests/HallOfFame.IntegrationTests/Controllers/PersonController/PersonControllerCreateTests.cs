using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TomskASUProject.HallOfFame.API;
using TomskASUProject.HallOfFame.API.DTOs;
using Xunit;

namespace TomskAsuProject.HallOfFame.IntegrationTests.Controllers
{
    public class PersonControllerCreateTests : PersonsControllerTestBase
    {
        public PersonControllerCreateTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreatePerson_With_Invalid_Person_Returns_BadRequest()
        {
            //Arrange
            var httpClient = _factory.CreateClient();
            var invalidPersonDto = GetPersonInvalid();

            //Act
            var httpResponse = await httpClient.PutAsJsonAsync<PersonDTO>("/api/v1/person/", invalidPersonDto);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);

            var modelState = JsonConvert.DeserializeObject<JObject>(await httpResponse.Content.ReadAsStringAsync());

            AssertModelStateInvalid(modelState);
        }

        [Fact]
        public async Task CreatePerson_With_Valid_Person_Creates_Person()
        {
            //Arrange
            var httpClient = _factory.CreateClient();
            var personValid = GetPersonFirst();

            //Act
            var httpResponse = await httpClient.PutAsJsonAsync<PersonDTO>("/api/v1/person/", personValid);

            //Assert
            httpResponse.EnsureSuccessStatusCode();

            var personCreated = JsonConvert.DeserializeObject<PersonResponseDTO>(await httpResponse.Content.ReadAsStringAsync());

            Assert.True(personCreated.Id > 0);
            AssertEqualPersons(personValid, personCreated);
        }

    }
}
