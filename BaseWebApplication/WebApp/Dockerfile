FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Install Node.js for TypeScript compilation
RUN apt-get update && apt-get install -y nodejs npm && rm -rf /var/lib/apt/lists/*
WORKDIR /src

COPY ["BaseWebApplication/WebApp/WebApp.csproj", "BaseWebApplication/WebApp/"]
RUN dotnet restore "BaseWebApplication/WebApp/WebApp.csproj"

COPY ["BaseWebApplication/WebApp/package.json", "BaseWebApplication/WebApp/"]
COPY ["BaseWebApplication/WebApp/package-lock.json", "BaseWebApplication/WebApp/"]
RUN cd BaseWebApplication/WebApp && npm install

COPY BaseWebApplication/WebApp/ BaseWebApplication/WebApp/
COPY Answer/src/Answer.Api/Protos/ Answer/src/Answer.Api/Protos/

RUN dotnet publish "BaseWebApplication/WebApp/WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]
