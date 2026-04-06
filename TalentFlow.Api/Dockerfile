# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use Render's PORT environment variable
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose a generic port (Render will override)
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

# 🔥 IMPORTANT: Make sure this path is correct in your repo
RUN dotnet restore TalentFlow.Api/TalentFlow.Api.csproj
RUN dotnet publish TalentFlow.Api/TalentFlow.Api.csproj -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TalentFlow.Api.dll"]