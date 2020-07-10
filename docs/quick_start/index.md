## Getting Started

Micro.Starter is an opinionated starter kit for .NET projects which aims to be batteries included but removable project.
Everything used on this project should be fairly easy to replace or remove completely. It aims to include most
of production needs which includes monitoring, linting, building docker images, etc

### Quick Start
```bash
git clone git@github.com:fossapps/Micro.Starter
cd Starter.Net
dotnet restore
dotnet run --project ./Micro.Starter.Api/Micro.Starter.Api.csproj
```
App should start listening on `http://localhost:5000`
