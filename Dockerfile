FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Haiku.slnx ./
COPY src/Haiku.Domain/*.csproj src/Haiku.Domain/
COPY src/Haiku.Infrastructure/*.csproj src/Haiku.Infrastructure/
COPY src/Haiku.Services/*.csproj src/Haiku.Services/
COPY src/Haiku.Web/*.csproj src/Haiku.Web/
COPY tests/Haiku.Tests/*.csproj tests/Haiku.Tests/
RUN dotnet restore

COPY . .
RUN dotnet test tests/Haiku.Tests/Haiku.Tests.csproj --configuration $BUILD_CONFIGURATION --no-restore
RUN dotnet publish src/Haiku.Web/Haiku.Web.csproj --configuration $BUILD_CONFIGURATION --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Haiku.Web.dll"]
