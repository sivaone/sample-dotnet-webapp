FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["sample-dotnet-webapp/sample-dotnet-webapp.csproj", "sample-dotnet-webapp/"]
RUN dotnet restore "sample-dotnet-webapp/sample-dotnet-webapp.csproj"
COPY . .
WORKDIR "/src/sample-dotnet-webapp"
RUN dotnet build "./sample-dotnet-webapp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./sample-dotnet-webapp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sample-dotnet-webapp.dll"]
