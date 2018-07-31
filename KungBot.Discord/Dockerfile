FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY KungBot.Discord/KungBot.Discord.csproj KungBot.Discord/
COPY Data/Data.csproj Data/
RUN dotnet restore KungBot.Discord/KungBot.Discord.csproj
COPY . .
WORKDIR /src/KungBot.Discord
RUN dotnet build KungBot.Discord.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish KungBot.Discord.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KungBot.Discord.dll"]
