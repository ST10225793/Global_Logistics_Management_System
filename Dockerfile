FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project configuration file
COPY ["Global Logistics Management System/Global Logistics Management System.csproj", "Global Logistics Management System/"]
RUN dotnet restore "Global Logistics Management System/Global Logistics Management System.csproj"

# Copy source assets
COPY . .
WORKDIR "/src/Global Logistics Management System"
RUN dotnet build "Global Logistics Management System.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Global Logistics Management System.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Global Logistics Management System.dll"]