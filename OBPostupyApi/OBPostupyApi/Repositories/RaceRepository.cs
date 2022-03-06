using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Contexts;
using OBPostupyApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            var controls = await _context.Controls.Where(c => c.CourseDataId == race.CourseData.Id).ToListAsync();
            var courses = await _context.Courses.Where(c => c.CourseDataId == race.CourseData.Id).ToListAsync();
            var splits = await _context.Splits.Where(c => c.CourseDataId == race.CourseData.Id).ToListAsync();

            _context.Controls.RemoveRange(controls);
            _context.Courses.RemoveRange(courses);
            _context.Splits.RemoveRange(splits);
            _context.Categories.RemoveRange(race.Categories);
            _context.Maps.RemoveRange(race.Maps);
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
    }
}
