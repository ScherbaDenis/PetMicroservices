FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Shared/Shared.Messaging/Shared.Messaging.csproj", "Shared/Shared.Messaging/"]
COPY ["Answer/src/Answer.Domain/Answer.Domain.csproj", "Answer/src/Answer.Domain/"]
COPY ["Answer/src/Answer.Application/Answer.Application.csproj", "Answer/src/Answer.Application/"]
COPY ["Answer/src/Answer.Infrastructure/Answer.Infrastructure.csproj", "Answer/src/Answer.Infrastructure/"]
COPY ["Answer/src/Answer.Api/Answer.Api.csproj", "Answer/src/Answer.Api/"]

RUN dotnet restore "Answer/src/Answer.Api/Answer.Api.csproj"

COPY Shared/Shared.Messaging/ Shared/Shared.Messaging/
COPY Answer/src/ Answer/src/

RUN dotnet publish "Answer/src/Answer.Api/Answer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Answer.Api.dll"]
