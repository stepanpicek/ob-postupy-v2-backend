using Microsoft.EntityFrameworkCore;
using OBPostupyApi.Contexts;
using OBPostupyApi.Entities;
using System.Threading.Tasks;

namespace OBPostupyApi.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly RepositoryContext _context;
        private const string INFO_KEY = "obpostupy_info";
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
    }
}
