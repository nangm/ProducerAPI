FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 5000
ENV RABBITMQ_HOST localhost
ENV RABBITMQ_PORT 5672 
ENV ASPNETCORE_URLS=http://+:5000


FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["ProducerAPI.csproj", "."]
RUN dotnet restore "./ProducerAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ProducerAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProducerAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProducerAPI.dll"]