FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Evalify.Api/Evalify.Api.csproj", "src/Evalify.Api/"]
COPY ["src/Evalify.Application/Evalify.Application.csproj", "src/Evalify.Application/"]
COPY ["src/Evalify.Domain/Evalify.Domain.csproj", "src/Evalify.Domain/"]
COPY ["src/Evalify.Infrastructure/Evalify.Infrastructure.csproj", "src/Evalify.Infrastructure/"]
RUN dotnet restore "src/Evalify.Api/Evalify.Api.csproj"
COPY src/ src/
WORKDIR "/src/src/Evalify.Api"
RUN dotnet build "./Evalify.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Evalify.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Evalify.Api.dll"]