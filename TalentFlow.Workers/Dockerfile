FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TalentFlow.Workers/*.csproj TalentFlow.Workers/
RUN dotnet restore TalentFlow.Workers/TalentFlow.Workers.csproj

COPY . .
RUN dotnet publish TalentFlow.Workers/TalentFlow.Workers.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TalentFlow.Workers.dll"]
