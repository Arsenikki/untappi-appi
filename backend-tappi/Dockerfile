FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["backend_tappi.csproj", "./"]

RUN dotnet restore "./backend_tappi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "backend_tappi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend_tappi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "backend_tappi.dll"]