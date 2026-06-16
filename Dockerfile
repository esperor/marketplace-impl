# ---- Build frontend ----
FROM node:20-alpine AS frontend-build
WORKDIR /src/marketplace_impl.client
ENV CI=true
COPY marketplace_impl.client/package*.json ./
RUN npm ci
COPY marketplace_impl.client/ .
RUN npm run build

# ---- Build & publish backend ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["marketplace_impl.Server/marketplace_impl.Server.csproj", "marketplace_impl.Server/"]
RUN dotnet restore "marketplace_impl.Server/marketplace_impl.Server.csproj"
COPY . .
WORKDIR /src/marketplace_impl.Server
RUN dotnet build "marketplace_impl.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:BuildProjectReferences=false
RUN dotnet publish "marketplace_impl.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -p:BuildingInsideDocker=true

# ---- Final runtime image ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=dotnet-build /app/publish .
COPY --from=frontend-build /src/marketplace_impl.client/dist ./wwwroot

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV RUNNING_IN_DOCKER=true
ENTRYPOINT ["dotnet", "marketplace_impl.Server.dll"]