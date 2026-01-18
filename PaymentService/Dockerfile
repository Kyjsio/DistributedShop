FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Shared/Shared.csproj Shared/
COPY PaymentService/PaymentService.csproj PaymentService/
RUN dotnet restore PaymentService/PaymentService.csproj

COPY . .
RUN dotnet publish PaymentService/PaymentService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","PaymentService.dll"]
