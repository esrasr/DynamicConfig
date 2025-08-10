# Use the official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ServiceA.Api/ServiceA.Api.csproj ServiceA.Api/
COPY DynamicConfig/DynamicConfig.csproj DynamicConfig/
COPY DynamicConfig.Web/DynamicConfig.Web.csproj DynamicConfig.Web/
RUN dotnet restore ServiceA.Api/ServiceA.Api.csproj
COPY . .
RUN dotnet restore
RUN dotnet publish ServiceA.Api/ServiceA.Api.csproj -c Release -o /app

# Use the official .NET 8 ASP.NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ServiceA.Api.dll"]