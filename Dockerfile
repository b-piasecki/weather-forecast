FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY *.sln .
COPY ./*/*.csproj .
RUN for file in ./*.csproj; do mkdir -p "./${file%.*}/" && mv "$file" "./${file%.*}/"; done
RUN dotnet restore "WeatherForecast.Api/WeatherForecast.Api.csproj"

COPY ./ .
RUN dotnet build "WeatherForecast.Api/WeatherForecast.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WeatherForecast.Api/WeatherForecast.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherForecast.Api.dll"]
