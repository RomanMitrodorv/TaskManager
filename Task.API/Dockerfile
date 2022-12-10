FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Task.API/Task.API.csproj", "Task.API/"]
RUN dotnet restore "Task.API/Task.API.csproj"
COPY . .
WORKDIR "/src/Task.API"
RUN dotnet build "Task.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Task.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Task.API.dll"]
