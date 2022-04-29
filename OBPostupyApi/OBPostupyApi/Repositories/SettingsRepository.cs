using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Contexts;
using OBPostupyApi.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly RepositoryContext _context;
        private const string INFO_KEY = "obpostupy_info";
        private const string FILE_KEY = "obpostupy_file";
        public SettingsRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Setting> GetInfo() => await _context.Settings.FirstOrDefaultAsync(s => s.Key == INFO_KEY);

        public async Task UpdateInfo(string info)
        {
            var oldInfo = await GetInfo();
            if (oldInfo != null)
            {
                oldInfo.Value = info;
            }
            else
            {
                await _context.Settings.AddAsync(new Setting
                {
                    Key = INFO_KEY,
                    Value = info
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveFile(string path)
        {
            await _context.Settings.AddAsync(new Setting
            {
                Key = FILE_KEY,
                Value = path
            });

            await _context.SaveChangesAsync();
        }

        public async Task<Setting> GetFile(int id) => await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);

        public async Task DeleteFile(int id)
        {
            var file = await GetFile(id);
            if(file != null)
            {
                _context.Settings.Remove(file);
            }
            await _context.SaveChangesAsync();
        }
        public async Task DeleteFile(Setting setting)
        {
            if (setting != null)
            {
                _context.Settings.Remove(setting);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Setting>> GetFiles()
        {
            return await _context.Settings
                .Where(s => s.Key == FILE_KEY)
                .ToListAsync();
        }
    }
}
