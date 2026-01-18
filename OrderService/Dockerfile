# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Shared/Shared.csproj Shared/
COPY OrderService/OrderService.csproj OrderService/
RUN dotnet restore OrderService/OrderService.csproj

COPY . .
RUN dotnet publish OrderService/OrderService.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","OrderService.dll"]
