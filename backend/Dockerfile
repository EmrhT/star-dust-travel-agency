FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS restore
WORKDIR /src
COPY src/StarDustTravelAgency.Api/StarDustTravelAgency.Api.csproj src/StarDustTravelAgency.Api/
RUN dotnet restore src/StarDustTravelAgency.Api/StarDustTravelAgency.Api.csproj

FROM restore AS build
COPY src/StarDustTravelAgency.Api/ src/StarDustTravelAgency.Api/
RUN dotnet publish src/StarDustTravelAgency.Api/StarDustTravelAgency.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "StarDustTravelAgency.Api.dll"]
