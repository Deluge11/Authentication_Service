FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

WORKDIR /src


COPY ["ConstantsLib_Versions/*.nupkg", "local-packages/"]

COPY ["Authentication_API/Authentication_API.csproj", "Authentication_API/"]
COPY ["Authentication_Application/Authentication_Application.csproj", "Authentication_Application/"]
COPY ["Authentication_Core/Authentication_Core.csproj", "Authentication_Core/"]
COPY ["Authentication_Infrastructure/Authentication_Infrastructure.csproj", "Authentication_Infrastructure/"]


RUN dotnet restore "Authentication_API/Authentication_API.csproj" \
    --source https://api.nuget.org/v3/index.json \
    --source /src/local-packages \
    --verbosity normal \
    --disable-parallel


COPY . .
WORKDIR "/src/Authentication_API"
RUN dotnet build "Authentication_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Authentication_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication_API.dll"]