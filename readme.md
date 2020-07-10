## Introduction
I'm starting this project so anyone can use it and kick start their project.
This will use .NET Core framework so it's truly cross-platform
(unless you make system calls to a specific platform)

Tests, builds and deployment will be done on linux machine, but windows should work too,
However a project as small as this shouldn't be a problem to support on all OSes.
CI will be done only on linux, and specially on a docker container. Since this is for starting microservices,
there's no need to test on windows, as long as docker image is built everything should be good.

## Comparison with dotnet generator
You might be tempted to think why can't we simply do `dotnet new webapi` and we obviously can for playing around.
I want to include much more features than just returning few values from `ValuesController` check below section to see what it'll contain.

## What will this contain?
As of now I see the following to be included on this kit:
- [x] docker support with docker-compose
- [x] background worker support
- [x] health check support
- [x] `ViewModel` to ensure communication is completely TypeSafe.
- [x] document controller response type for swagger
- [x] Swagger Docs (`spec.json` endpoint and docs)
- [ ] Linting
- [x] Docker image generation after doing CI testing
- [ ] Logging (probably with kibana, but maybe using interface instead)
- [x] Health Checks
- [ ] Caching

## Timeline
Well, this isn't a full time project which I can work on, so the more contribution I get the faster project will flow.

I'm thrilled about this project and want to use for myself anyway, there'll be continuous development.
Further contribution will only make this grow better and faster.
I want to see this project be production ready, where you clone it,
and start writing production ready code without setting up

after a while no more development will be needed on this project, but rather other services will be included

## Used packages

## Getting Started
Simply clone this repository, and open in Rider or Visual Studio Code and start building.

## Contribution
Please follow the existing coding standards which is being followed, no trailing whitespaces, edge cases goes to if conditions,
follow line of sight rule. Happy path is always straight down, only short circuit (early exits) the error path unless there's a strong reason not to.

## More Documentation
Before a feature is worked on, I'll try to document on what needs to be done, after the feature is ready,
I'll try to finalize the documentation. If there's something missing, please contribute.
