FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /src
COPY ["Core/Core.csproj", "Core/"]
COPY ["UsersApi/UsersApi.csproj", "UsersApi/"]
RUN dotnet restore "UsersApi/UsersApi.csproj"
COPY . .
WORKDIR "/src/UsersApi"
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "UsersApi.dll"]