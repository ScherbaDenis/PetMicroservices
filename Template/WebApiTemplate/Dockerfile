FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Shared/Shared.Messaging/Shared.Messaging.csproj", "Shared/Shared.Messaging/"]
COPY ["Template/Template.Domain/Template.Domain.csproj", "Template/Template.Domain/"]
COPY ["Template/Template.Service/Template.Service.csproj", "Template/Template.Service/"]
COPY ["Template/Template.DataAccess/Template.DataAccess.MsSql.csproj", "Template/Template.DataAccess/"]
COPY ["Template/WebApiTemplate/WebApiTemplate.csproj", "Template/WebApiTemplate/"]

RUN dotnet restore "Template/WebApiTemplate/WebApiTemplate.csproj"

COPY Shared/Shared.Messaging/ Shared/Shared.Messaging/
COPY Template/ Template/

RUN dotnet publish "Template/WebApiTemplate/WebApiTemplate.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApiTemplate.dll"]
