FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /src
COPY ["Core/Core.csproj", "Core/"]
COPY ["TasksApi/TasksApi.csproj", "TasksApi/"]
RUN dotnet restore "TasksApi/TasksApi.csproj"
COPY . .
WORKDIR "/src/TasksApi"
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "TasksApi.dll"]