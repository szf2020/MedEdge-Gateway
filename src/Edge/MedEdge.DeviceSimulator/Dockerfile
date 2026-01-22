FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["src/Edge/MedEdge.DeviceSimulator/MedEdge.DeviceSimulator.csproj", "src/Edge/MedEdge.DeviceSimulator/"]

RUN dotnet restore "src/Edge/MedEdge.DeviceSimulator/MedEdge.DeviceSimulator.csproj"

COPY . .

WORKDIR "/src/src/Edge/MedEdge.DeviceSimulator"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MedEdge.DeviceSimulator.dll"]
