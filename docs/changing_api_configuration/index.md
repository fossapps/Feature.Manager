# Default configuration
The default configuration when you run this service can be found at appsettings.json file on `Micro.KeyStore.Api` project

## Overwriting default configuration
There are two ways to overwrite default configuration:

### Using a separate appsettings.[env].json
When you deploy this service, the environment is most likely going to be `prod`, run the service using the folllowing environment variable: `ASPNETCORE_ENVIRONMENT` which should be set to `prod`
then you can simply clone the `appsettings.json` into `appsettings.prod.json` and overwrite the values there.

Please look into how .NET handles settings merging to learn more.

Before deploying simply mount appsettings.prod.json into the docker's file system.

You can even write your Dockerfile which starts with `fossapps/micro.starter` and simply copy your values into the image. Make sure you watch for new updates.

### Using Environment variables
In case you don't want to use separate files you can also overwrite files using environment variables.

A sample of `appsettings.json` is below:
```json
{
  "DatabaseConfig": {
      "Host": "localhost",
      "Port": 15433,
      "Name": "starter_db",
      "User": "starter",
      "Password": "secret"
    }
}
```
If you'd like to change a particular you can always use environment variables to overwrite the default values like the following: {key}(double _){key}="value" example to change database config:
```bash
DatabaseConfig__Host="<your host>"
DatabaseConfig__Port="5433"
DatabaseConfig__Name="DbName"
```

If you're running with CLI on linux, simply keep a spaces between each of them, and for windows, go figure.
