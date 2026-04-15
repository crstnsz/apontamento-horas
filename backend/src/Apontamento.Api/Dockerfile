# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Apontamento.Api.csproj", "."]
COPY ["../Apontamento.Domain/Apontamento.Domain.csproj", "../Apontamento.Domain/"]
COPY ["../Apontamento.Infrastructure.MongoDb/Apontamento.Infrastructure.MongoDb.csproj", "../Apontamento.Infrastructure.MongoDb/"]
RUN dotnet restore "Apontamento.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "Apontamento.Api.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "Apontamento.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Apontamento.Api.dll"]