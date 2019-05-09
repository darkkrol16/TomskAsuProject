using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TomskASUProject.HallOfFame.API.DTOs;
using TomskASUProject.HallOfFame.API.Infrastructure.Repositories;
using TomskASUProject.HallOfFame.API.Models;

namespace TomskASUProject.HallOfFame.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{apiVersion:apiVersion}/person")]
    public class PersonsController : Controller
    {
        private readonly ILogger<PersonsController> _logger;
        private readonly IPersonRepository _personRepository;

        public PersonsController(ILogger<PersonsController> logger, IPersonRepository personRepository)
        {
            _logger = logger;
            _personRepository = personRepository;
        }


        [HttpGet("~/api/v{apiVersion:apiVersion}/persons")]
        [ProducesResponseType(typeof(IEnumerable<PersonResponseDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<PersonResponseDTO>>> GetPersonsAsync ()
        {
            var personsList = await _personRepository.GetPersonsAsync();

            var persons = personsList.Select(e => new PersonResponseDTO(e));

            return Ok(persons);
        }


        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(PersonResponseDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PersonResponseDTO>> GetPersonAsync([FromRoute]long id)
        {
            var person = await _personRepository.GetPersonAsync(id);

            if (person == null)
                return NotFound($"Person with id {id} not found");

            var personDto = new PersonResponseDTO(person);
            return Ok(personDto);
        }


        [HttpPut("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<PersonResponseDTO>> CreatePersonAsync([FromBody] PersonDTO personDto)
        {
            if (ModelState.IsValid) { 
                var person = new Person
                {
                    Name = personDto.Name,
                    DisplayName = personDto.DisplayName,
                };

                AddSkills(person, personDto);

                await _personRepository.AddPersonAsync(person);

                var createdPersonDto = new PersonResponseDTO(person);
                return Ok(createdPersonDto);
            }

            return BadRequest(ModelState);
        }


        [HttpPost("{id:long}")]
        [ProducesResponseType(typeof(PersonResponseDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<PersonResponseDTO>> UpdatePersonAsync([FromBody]PersonDTO personDto, [FromRoute]long id)
        {
            var person = await _personRepository.GetPersonAsync(id);

            if (person == null)
                return NotFound($"Person with id {id} not found");
            if (ModelState.IsValid)
            {
                person.Name = personDto.Name;
                person.DisplayName = personDto.DisplayName;
                person.Skills.RemoveAll(e => e.PersonId == person.Id);

                AddSkills(person, personDto);

                await _personRepository.UpdateAsync();

                var updatedPersonDto = new PersonResponseDTO(person);

                return Ok(updatedPersonDto);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(PersonResponseDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PersonResponseDTO>> DeletePersonAsync([FromRoute] long id)
        {
            var person = await _personRepository.GetPersonAsync(id);

            if (person == null)
                return NotFound($"Person with id {id} not found");

            await _personRepository.RemovePersonAsync(person);
            
            var personDto = new PersonResponseDTO(person);
            return Ok(personDto);
        }

        [NonAction]
        public void AddSkills(Person person, PersonDTO personDto)
        {
            foreach (var skillDto in personDto.Skills)
            {
                var skill = new Skill()
                {
                    Name = skillDto.Name,
                    Level = skillDto.Level,
                    PersonId = person.Id
                };

                person.Skills.Add(skill);
            }
        }
    }
}
