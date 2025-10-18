# ---------- base images ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ---------- copy csproj'ler (restore cache için) ----------
COPY Config.Api/Config.Api.csproj ./Config.Api/
COPY Config.Application/Config.Application.csproj ./Config.Application/
COPY Config.Domain/Config.Domain.csproj ./Config.Domain/
COPY Config.Persistence/Config.Persistence.csproj ./Config.Persistence/
COPY Config.Infrastructure/Config.Infrastructure.csproj ./Config.Infrastructure/

RUN dotnet restore ./Config.Api/Config.Api.csproj

# ---------- tüm kaynakları kopyala ve publish ----------
COPY . .
WORKDIR /src/Config.Api
RUN dotnet publish -c Release -o /out

# ---------- final ----------
FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Config.Api.dll"]
