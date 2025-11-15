FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /out .

COPY ./Init.sql /docker-entrypoint-initdb.d/Init.sql

EXPOSE 8000
ENTRYPOINT ["dotnet", "OwlEdu-Manager-Server.dll"]
