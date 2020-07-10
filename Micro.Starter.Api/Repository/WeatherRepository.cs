using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micro.Starter.Api.Models;
using Micro.Starter.Api.Uuid;
using Microsoft.EntityFrameworkCore;

namespace Micro.Starter.Api.Repository
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly ApplicationContext _db;
        private readonly IUuidService _uuid;

        public WeatherRepository(ApplicationContext db, IUuidService uuid)
        {
            _db = db;
            _uuid = uuid;
        }

        public async Task<IEnumerable<Weather>> GetAll()
        {
            return (await _db.Weathers.ToListAsync()).AsEnumerable();
        }

        public Task<Weather> FindById(string id)
        {
            return _db.Weathers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Weather> Create(Weather weather)
        {
            weather.Id = _uuid.GenerateUuId();
            var result = await _db.Weathers.AddAsync(weather);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task Delete(string id)
        {
            var entities = _db.Weathers.Where(w => w.Id == id);
            _db.Weathers.RemoveRange(entities);
            await _db.SaveChangesAsync();
        }
    }
}
