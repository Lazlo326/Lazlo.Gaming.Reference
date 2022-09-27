#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Lazlo.ICS.Reference.Worker/Lazlo.ICS.Reference.Worker.csproj", "Lazlo.ICS.Reference.Worker/"]
RUN dotnet restore "Lazlo.ICS.Reference.Worker/Lazlo.ICS.Reference.Worker.csproj"
COPY . .
WORKDIR "/src/Lazlo.ICS.Reference.Worker"
RUN dotnet build "Lazlo.ICS.Reference.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lazlo.ICS.Reference.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lazlo.ICS.Reference.Worker.dll"]