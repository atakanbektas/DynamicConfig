FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# (mvc projesi başka projelere referans vermiyorsa sadece kendisini kopyalamak yeterli)
COPY Admin.Mvc/Admin.Mvc.csproj ./Admin.Mvc/
RUN dotnet restore ./Admin.Mvc/Admin.Mvc.csproj

COPY . .
WORKDIR /src/Admin.Mvc
RUN dotnet publish -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Admin.Mvc.dll"]
