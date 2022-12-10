FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Telegram.API/Telegram.API.csproj", "Telegram.API/"]
RUN dotnet restore "Telegram.API/Telegram.API.csproj"
COPY . .
WORKDIR "/src/Telegram.API"
RUN dotnet build "Telegram.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Telegram.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Telegram.API.dll"]
