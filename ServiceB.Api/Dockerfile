# Use the official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ServiceB.Api/ServiceB.Api.csproj ServiceB.Api/
COPY DynamicConfig/DynamicConfig.csproj DynamicConfig/
COPY DynamicConfig.Web/DynamicConfig.Web.csproj DynamicConfig.Web/
RUN dotnet restore ServiceB.Api/ServiceB.Api.csproj
COPY . .
RUN dotnet restore
RUN dotnet publish ServiceB.Api/ServiceB.Api.csproj -c Release -o /app

# Use the official .NET 8 ASP.NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ServiceB.Api.dll"]
