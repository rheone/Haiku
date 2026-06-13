FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Haiku.slnx ./
COPY src/Haiku.Domain/*.csproj src/Haiku.Domain/
COPY src/Haiku.Infrastructure/*.csproj src/Haiku.Infrastructure/
COPY src/Haiku.Services/*.csproj src/Haiku.Services/
COPY src/Haiku.Web/*.csproj src/Haiku.Web/
COPY src/MicroMediator/*.csproj src/MicroMediator/
COPY tests/Haiku.Tests/*.csproj tests/Haiku.Tests/
COPY tests/Haiku.Domain.Tests/*.csproj tests/Haiku.Domain.Tests/
COPY tests/Haiku.Services.Tests/*.csproj tests/Haiku.Services.Tests/
COPY tests/Haiku.Infrastructure.Tests/*.csproj tests/Haiku.Infrastructure.Tests/
COPY tests/Haiku.Web.Tests/*.csproj tests/Haiku.Web.Tests/
COPY tests/MicroMediator.Tests/*.csproj tests/MicroMediator.Tests/
RUN dotnet restore

COPY . .
RUN dotnet test Haiku.slnx --configuration $BUILD_CONFIGURATION --no-restore
RUN dotnet publish src/Haiku.Web/Haiku.Web.csproj --configuration $BUILD_CONFIGURATION --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Haiku.Web.dll"]
