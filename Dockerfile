FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY . ./

RUN dotnet restore

RUN dotnet build "MenuDigitalRestaurante/MenuDigitalRestaurante.csproj"  -c Release -o /app/build

RUN dotnet publish "MenuDigitalRestaurante/MenuDigitalRestaurante.csproj" -c Release -o /app/publish 

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build-env /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "MenuDigitalRestaurante.dll"]