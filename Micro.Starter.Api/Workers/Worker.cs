using System;
using System.Threading;
using System.Threading.Tasks;
using Micro.Starter.Api.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Micro.Starter.Api.Workers
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWeatherRepository _weatherRepository;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _weatherRepository = serviceScopeFactory.CreateScope().ServiceProvider
                .GetRequiredService<IWeatherRepository>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at {DateTime.Now}");
                foreach (var weather in await _weatherRepository.GetAll())
                {
                    _logger.LogInformation($"removing {weather.Id}");
                    await _weatherRepository.Delete(weather.Id);
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
