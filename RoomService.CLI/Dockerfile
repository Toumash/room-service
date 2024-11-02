FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RoomService.CLI/RoomService.CLI.csproj", "RoomService.CLI/"]
COPY ["RoomService.Application/RoomService.Application.csproj", "RoomService.Application/"]
RUN dotnet restore "RoomService.CLI/RoomService.CLI.csproj"
COPY . .
WORKDIR "/src/RoomService.CLI"
RUN dotnet build "RoomService.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RoomService.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoomService.CLI.dll"]
