FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

USER app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MenuDigitalRestaurante/MenuDigitalRestaurante.csproj", "MenuDigitalRestaurante/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "MenuDigitalRestaurante/MenuDigitalRestaurante.csproj"


COPY . .
WORKDIR "/src/MenuDigitalRestaurante"
RUN dotnet build "MenuDigitalRestaurante.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MenuDigitalRestaurante.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MenuDigitalRestaurante.dll"]