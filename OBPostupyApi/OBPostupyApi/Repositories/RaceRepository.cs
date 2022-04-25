using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Contexts;
using OBPostupyApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace OBPostupyApi.Repositories
{
    public class RaceRepository : IRaceRepository
    {
        private readonly RepositoryContext _context;

        public RaceRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Race> GetRaceByKeyAsync(string key)
        {
            return await _context.Races.Where(r => r.Key == key).SingleOrDefaultAsync();
        }

        public async Task<List<Race>> GetAllPublicRacesAsync()
        {
            return await _context.Races.Where(r => r.Type == RaceType.Public).ToListAsync();
        }

        public async Task<List<Race>> GetAllOrisRacesInMonthAsync()
        {
            return await _context.Races.Where(r => r.Type == RaceType.Public && r.OrisId != 0 && r.StartTime > DateTime.Now.AddMonths(-1)).ToListAsync();
        }

        public async Task<List<Race>> GetAllUserRacesAsync(string id)
        {
            return await _context.Races.Where(r => r.UserId == id).ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesAsync(string key)
        {
            return await _context.Races
                .Where(r => r.Key == key)
                .Include(r => r.Categories)
                .ThenInclude(c => c.PersonResults)
                .ThenInclude(p => p.Person)
                .Select(r => r.Categories)
                .SingleOrDefaultAsync();
        }

        public async Task DeleteRaceAsync(string key)
        {
            Race race = await _context.Races.Where(r => r.Key == key)
                .Include(r => r.CourseData)
                .Include(r => r.Categories)
                .Include(r => r.Maps)
                .SingleOrDefaultAsync();
            
            if (race?.CourseData != null)
            {
                var controls = await _context.Controls.Where(c => c.CourseDataId == race.CourseData.Id).ToListAsync();
                var courses = await _context.Courses.Where(c => c.CourseDataId == race.CourseData.Id).ToListAsync();
                var splits = await _context.Splits.Where(c => c.CourseDataId == race.CourseData.Id).ToListAsync();

                _context.Controls.RemoveRange(controls);
                _context.Courses.RemoveRange(courses);
                _context.Splits.RemoveRange(splits);
            }
            if (race?.Categories != null) _context.Categories.RemoveRange(race.Categories);
            if (race?.Maps != null) _context.Maps.RemoveRange(race.Maps);
            _context.Races.Remove(race);

            await SaveAsync();
        }

        public async Task CreateRaceAsync(Race race)
        {
            if (race == null) throw new ArgumentNullException(nameof(race));
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            _context.Races.Update(race);

            await Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Race>> GetAllUserRacesByRegNumberAsync(string regNumber)
        {
            return await _context.Person
                .FromSqlRaw($"SELECT * FROM Person WHERE RegNumbers = '{regNumber}'")
                .Include(p => p.PersonResults)
                .ThenInclude(pp => pp.Category)
                .ThenInclude(c => c.Race)
                .SelectMany(p => p.PersonResults.Select(pp => pp.Category).Select(c => c.Race))
                .Where(r => r.Type == RaceType.Public)
                .ToListAsync();
        }

        public async Task<List<Race>> GetAllRacesAsync()
        {
            return await _context.Races
                .Include(r => r.User)
                .ToListAsync();
        }
    }
}
