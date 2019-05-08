using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TomskASUProject.HallOfFame.API.Controllers.V1;
using TomskASUProject.HallOfFame.API.DTOs;
using TomskASUProject.HallOfFame.API.Infrastructure.Repositories;
using TomskASUProject.HallOfFame.API.Models;
using Xunit;

namespace TomskAsuProject.HallOfFame.UnitTests
{
    public class PersonsControllerTest
    {
        private readonly Mock<IPersonRepository> _personRepositoryMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;

        public PersonsControllerTest()
        {
            _personRepositoryMock = new Mock<IPersonRepository>();
            _loggerMock = new Mock<ILogger<PersonsController>>();
        }

        [Fact]
        public async Task GetPerson_Success()
        {
            //Arrange
            var fakePersonId = 1L;
            var fakePerson = GetPersonFake(fakePersonId);
            var fakePersonDto = GetPersonResponseDtoFake(fakePerson);

            _personRepositoryMock.Setup(x => x.GetPersonAsync(It.IsAny<long>()))
                .Returns(Task.FromResult(fakePerson));

            //Act
            var personsController = new PersonsController(_loggerMock.Object, _personRepositoryMock.Object);
            var actionResult = await personsController.GetPersonAsync(fakePersonId);

            //Assert
            var result = actionResult.Result as OkObjectResult;
            var resultValue = (PersonResponseDTO)result.Value;

            Assert.Equal(result.StatusCode, (int)HttpStatusCode.OK);
            Assert.Equal(resultValue.Id, fakePersonDto.Id);
            Assert.Equal(resultValue.Name, fakePersonDto.Name);
            Assert.Equal(resultValue.DisplayName, fakePersonDto.DisplayName);

            for (int i = 0; i < fakePersonDto.Skills.Count; i++) { 
                Assert.Equal(fakePersonDto.Skills[i].Name, resultValue.Skills[i].Name);
                Assert.Equal(fakePersonDto.Skills[i].Level, resultValue.Skills[i].Level);
            }
        }

        [Fact]
        public async Task GetPerson_When_User_Is_Not_Found_Will_Return_Not_Found()
        {
            //Arrange
            var fakePersonId = 0;

            _personRepositoryMock.Setup(e => e.GetPersonAsync(fakePersonId))
                .Returns(Task.FromResult<Person>(null));

            //Act
            var personsController = new PersonsController(_loggerMock.Object, _personRepositoryMock.Object);
            var actionResult = await personsController.GetPersonAsync(fakePersonId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task CreatePerson_When_Model_Is_Not_Valid_Will_Return_Error_State()
        {
            //Arrange
            var fakePersonId = 0;
            var fakePerson = GetPersonFakeInvalid(fakePersonId);
            var fakePersonDto = GetPersonDtoFake(fakePerson);

            //Act
            var personsController = new PersonsController(_loggerMock.Object, _personRepositoryMock.Object);
            personsController.ModelState.AddModelError("Name", "The field Name must be a string or array type with a minimum length of '3'.");
            personsController.ModelState.AddModelError("Skills[0].Level", "The field Level must be between 0 and 10.");
            var actionResult = await personsController.CreatePersonAsync(fakePersonDto);


            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdatePerson_When_User_Is_Not_Found_Will_Return_Not_Found()
        {
            //Arrange
            var fakePersonId = 0;
            var fakePerson = GetPersonFake(fakePersonId);
            var fakePersonDto = GetPersonDtoFake(fakePerson);

            _personRepositoryMock.Setup(e => e.GetPersonAsync(fakePersonId))
                .Returns(Task.FromResult<Person>(null));

            //Act
            var personsController = new PersonsController(_loggerMock.Object, _personRepositoryMock.Object);
            var actionResult = await personsController.UpdatePersonAsync(fakePersonDto,0);

            //Assert
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task UpdatePerson_When_Model_Is_Not_Valid_Will_Return_Error_State()
        {
            //Arrange
            var fakePersonId = 0;
            var fakePerson = GetPersonFakeInvalid(fakePersonId);
            var fakePersonDto = GetPersonDtoFake(fakePerson);

            _personRepositoryMock.Setup(e => e.GetPersonAsync(fakePersonId))
                .Returns(Task.FromResult(fakePerson));

            //Act
            var personsController = new PersonsController(_loggerMock.Object, _personRepositoryMock.Object);
            personsController.ModelState.AddModelError("Name", "The field Name must be a string or array type with a minimum length of '3'.");
            personsController.ModelState.AddModelError("Skills[0].Level", "The field Level must be between 0 and 10.");
            var actionResult = await personsController.UpdatePersonAsync(fakePersonDto, fakePersonId);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }


        [Fact]
        public async Task DeletePerson_When_User_Is_Not_Found_Will_Return_Not_Found()
        {
            //Arrange
            var fakePersonId = 0;
            var fakePerson = GetPersonFake(fakePersonId);

            _personRepositoryMock.Setup(e => e.GetPersonAsync(fakePersonId))
                .Returns(Task.FromResult<Person>(null));
            _personRepositoryMock.Setup(e => e.RemovePersonAsync(fakePerson))
                .Returns(Task.CompletedTask);
            //Act
            var personsController = new PersonsController(_loggerMock.Object, _personRepositoryMock.Object);
            var actionResult = await personsController.DeletePersonAsync(fakePersonId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeletePerson_When_User_Is_Deleted_Will_Return_User()
        {
            //Arrange
            var fakePersonId = 0;
            var fakePerson = GetPersonFake(fakePersonId);
            var fakePersonDto = GetPersonResponseDtoFake(fakePerson);

            _personRepositoryMock.Setup(e => e.GetPersonAsync(fakePersonId))
                .Returns(Task.FromResult(fakePerson));
            _personRepositoryMock.Setup(e => e.RemovePersonAsync(fakePerson))
                .Returns(Task.CompletedTask);

            //Act
            var personsController = new PersonsController(_loggerMock.Object, _personRepositoryMock.Object);
            var actionResult = await personsController.DeletePersonAsync(fakePersonId);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var resultValue = (PersonResponseDTO)okObjectResult.Value;

            Assert.Equal(resultValue.Id, fakePersonDto.Id);
            Assert.Equal(resultValue.Name, fakePersonDto.Name);
            Assert.Equal(resultValue.DisplayName, fakePersonDto.DisplayName);

            for (int i = 0; i < fakePersonDto.Skills.Count; i++)
            {
                Assert.Equal(fakePersonDto.Skills[i].Name, resultValue.Skills[i].Name);
                Assert.Equal(fakePersonDto.Skills[i].Level, resultValue.Skills[i].Level);
            }

        }


        public PersonResponseDTO GetPersonResponseDtoFake(Person person) {
            return new PersonResponseDTO(person);
        }

        public PersonDTO GetPersonDtoFake(Person person)
        {
            return new PersonDTO(person);
        }

        public Person GetPersonFake(long id)
        {
            return new Person
            {
                Id = id,
                Name = "Test",
                DisplayName = "TestUser",
                Skills = new List<Skill>
                {
                    new Skill
                    {
                        PersonId = id,
                        Name = "Ten",
                        Level = 10
                    },
                    new Skill
                    {
                        PersonId = id,
                        Name = "Nine",
                        Level = 9
                    }
                }
            };
        }

        public Person GetPersonFakeInvalid(int id)
        {
            return new Person
            {
                Id = id,
                Name = "1", //Name min length in 3
                DisplayName = "TestUser",
                Skills = new List<Skill>
                {
                    new Skill
                    {
                        PersonId = 0,
                        Name = "Ten",
                        Level = 20 //Max level is 10
                    },
                }
            };
        }
    }
}
