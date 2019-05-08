using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomskASUProject.HallOfFame.API.Infrastructure.Contexts;
using TomskASUProject.HallOfFame.API.Models;

namespace TomskASUProject.HallOfFame.API.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person> GetPersonAsync(long id)
        {
            return await _context.Persons
                .Include(e => e.Skills)
                .SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Person>> GetPersonsAsync()
        {
            return await _context.Persons
                .ToListAsync();
        }

        public async Task AddPersonAsync(Person person)
        {
            _context.Persons.Add(person);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemovePersonAsync(Person person)
        {
            _context.Persons.Remove(person);

            await _context.SaveChangesAsync();
        }
    }
}
