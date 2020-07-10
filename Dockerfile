FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine3.9 as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out --self-contained --runtime linux-x64 ./Feature.Manager.Api/Feature.Manager.Api.csproj

FROM debian:jessie-slim

ARG VERSION
ENV APP_VERSION="${VERSION}"
WORKDIR /app
COPY --from=build /app/out/ ./
RUN chmod +x ./Feature.Manager.Api && apt-get update && apt-get install -y --no-install-recommends libicu-dev openssl && rm -Rf /var/lib/apt/lists/* && apt-get clean
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
STOPSIGNAL SIGTERM
CMD ["./Feature.Manager.Api"]
