using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomskASUProject.HallOfFame.API.Models;

namespace TomskASUProject.HallOfFame.API.Infrastructure.Repositories
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetPersonsAsync();

        Task<Person> GetPersonAsync(long id);

        Task AddPersonAsync(Person person);

        Task UpdateAsync();

        Task RemovePersonAsync(Person person);
    }
}
