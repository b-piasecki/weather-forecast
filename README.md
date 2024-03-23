# Weather Forecast

## API

### Docker

1. Create a `.env` file:

```dotenv
ConnectionStrings__WeatherForecastDatabase=[connection string]
ASPNETCORE_ENVIRONMENT="Development"
ASPNETCORE_HTTPS_PORTS="443"
ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
ASPNETCORE_Kestrel__Certificates__Default__Password=${CERT_PASSWORD}
```

2. Run `docker-compose up -d`

### IDE

1. Create user secrets with following content:

```json
{
    "ConnectionStrings": {
        "WeatherForecastDatabase": "[connection string]"
    }
}
```