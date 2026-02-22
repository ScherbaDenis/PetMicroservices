FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Shared/Shared.Messaging/Shared.Messaging.csproj", "Shared/Shared.Messaging/"]
COPY ["Comment/Comment.Domain/Comment.Domain.csproj", "Comment/Comment.Domain/"]
COPY ["Comment/Comment.Service/Comment.Service.csproj", "Comment/Comment.Service/"]
COPY ["Comment/Comment.DataAccess.MsSql/Comment.DataAccess.MsSql.csproj", "Comment/Comment.DataAccess.MsSql/"]
COPY ["Comment/WebApiComment/WebApiComment.csproj", "Comment/WebApiComment/"]

RUN dotnet restore "Comment/WebApiComment/WebApiComment.csproj"

COPY Shared/Shared.Messaging/ Shared/Shared.Messaging/
COPY Comment/ Comment/

RUN dotnet publish "Comment/WebApiComment/WebApiComment.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApiComment.dll"]
