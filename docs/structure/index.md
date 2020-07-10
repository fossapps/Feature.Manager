## File Structure
```
$ tree -I 'docs|*bin*|*obj*'
.
├── docker-compose.yml
├── Dockerfile
├── hooks
│   └── commit-msg
├── Micro.Starter.Api
│   ├── appsettings.json
│   ├── Configs
│   │   ├── DatabaseConfig.cs
│   │   └── SlackLoggingConfig.cs
│   ├── Controllers
│   │   ├── WeatherForecastController.cs
│   │   └── WeatherForecast.cs
│   ├── HealthCheck
│   │   ├── HealthCheckController.cs
│   │   └── HealthData.cs
│   ├── Micro.Starter.Api.csproj
│   ├── Migrations
│   ├── Models
│   │   ├── ApplicationContext.cs
│   │   └── Weather.cs
│   ├── Program.cs
│   ├── Properties
│   │   └── launchSettings.json
│   ├── Repository
│   │   ├── IWeatherRepository.cs
│   │   └── WeatherRepository.cs
│   ├── Startup.cs
│   ├── Uuid
│   │   ├── IUuidService.cs
│   │   └── UuidService.cs
│   └── Workers
│       └── Worker.cs
├── Micro.Starter.sln
├── Micro.Starter.UnitTest
│   ├── ExternalTests
│   │   └── postman_tests.sh
│   ├── Micro.Starter.UnitTest.csproj
│   └── UnitTest1.cs
└── release.config.js
```

## Micro.Starter.Api
This is the project which is actually booted, once it boots, it configures and starts listening for incoming requests.
`Controllers` are where requests will land in, they're not supposed to contain any business logic,
but rather extract data from requests and pass in to other services.

## Micro.Starter.UnitTest
This project contains unit tests and postman tests for Micro.Starter.Api
