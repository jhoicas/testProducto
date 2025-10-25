# Imagen multi-stage para .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj y restaurar
COPY src/Domain/ProductApi.Domain.csproj src/Domain/
COPY src/Application/ProductApi.Application.csproj src/Application/
COPY src/Infrastructure/ProductApi.Infrastructure.csproj src/Infrastructure/
COPY src/Api/ProductApi.csproj src/Api/

RUN dotnet restore src/Api/ProductApi.csproj

# Copiar todo y compilar
COPY . .
RUN dotnet build src/Api/ProductApi.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish src/Api/ProductApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ProductApi.dll"]
