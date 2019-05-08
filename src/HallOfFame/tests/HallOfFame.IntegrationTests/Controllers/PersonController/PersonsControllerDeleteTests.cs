using Newtonsoft.Json;
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
    public class PersonsControllerDeleteTests : PersonsControllerTestBase
    {
        public PersonsControllerDeleteTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeletePerson_With_Invalid_Id_Returns_NotFound()
        {
            //Arrange
            var httpClient = _factory.CreateClient();

            //Act
            var httpResponse = await httpClient.DeleteAsync("/api/v1/person/0");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_Person_With_Valid_Id_Returns_Person_And_Ok()
        {
            var httpClient = _factory.CreateClient();
            var personNew = GetPersonFirst();

            //The new user created to not break other test
            var httpCreateResponse = await httpClient.PutAsJsonAsync<PersonDTO>("/api/v1/person/", personNew);
            httpCreateResponse.EnsureSuccessStatusCode();
            var personCreated = JsonConvert.DeserializeObject<PersonResponseDTO>(await httpCreateResponse.Content.ReadAsStringAsync());
            var httpDeleteResponse = await httpClient.DeleteAsync("/api/v1/person/" + personCreated.Id);
            var personDeleted = JsonConvert.DeserializeObject<PersonResponseDTO>(await httpCreateResponse.Content.ReadAsStringAsync());

            httpDeleteResponse.EnsureSuccessStatusCode();
            Assert.True(personDeleted.Id > 0);
            AssertEqualPersons(personNew, personDeleted);
        }
    }
}
