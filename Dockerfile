FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore Api/Api.csproj

COPY . ./
RUN dotnet publish Api/Api.csproj -c Release -o ../out

FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "Api.dll"]