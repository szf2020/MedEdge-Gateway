FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "./"]
COPY ["src/Edge/MedEdge.EdgeGateway/MedEdge.EdgeGateway.csproj", "src/Edge/MedEdge.EdgeGateway/"]

RUN dotnet restore "src/Edge/MedEdge.EdgeGateway/MedEdge.EdgeGateway.csproj"

COPY . .

WORKDIR "/src/src/Edge/MedEdge.EdgeGateway"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MedEdge.EdgeGateway.dll"]
