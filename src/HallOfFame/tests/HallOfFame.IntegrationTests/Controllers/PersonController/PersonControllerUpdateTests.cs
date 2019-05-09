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
    public class PersonControllerUpdateTests : PersonsControllerTestBase
    {
        public PersonControllerUpdateTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdatePerson_With_Invalid_Id_Returns_NotFound()
        {
            //Arrange
            var httpClient = _factory.CreateClient();
            var person = GetPersonFirst();

            //Act
            var httpResponse = await httpClient.PostAsJsonAsync("/api/v1/person/0", person);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_With_Invalid_ModelState_Returns_BadRequest()
        {
            //Arrange
            var httpClient = _factory.CreateClient();
            var invalidPersonDto = GetPersonInvalid();

            //Act
            var httpResponse = await httpClient.PostAsJsonAsync<PersonDTO>("/api/v1/person/1", invalidPersonDto);


            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);

            var modelState = JsonConvert.DeserializeObject<JObject>(await httpResponse.Content.ReadAsStringAsync());

            AssertModelStateInvalid(modelState);
        }
    }
}
