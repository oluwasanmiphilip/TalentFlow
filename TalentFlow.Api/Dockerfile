# Base runtime image (ASP.NET Core 10)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# Respect environment variable PORT (Render sets this dynamically)
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE 80
EXPOSE 443

# Build stage (SDK 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj first for better caching
COPY TalentFlow.Api/*.csproj TalentFlow.Api/
RUN dotnet restore TalentFlow.Api/TalentFlow.Api.csproj

# Copy everything else and publish
COPY . .
RUN dotnet publish TalentFlow.Api/TalentFlow.Api.csproj -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TalentFlow.Api.dll"]
