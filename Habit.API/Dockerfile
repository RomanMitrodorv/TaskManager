FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Habit.API/Habit.API.csproj", "Habit.API/"]
RUN dotnet restore "Habit.API/Habit.API.csproj"
COPY . .
WORKDIR "/src/Habit.API"
RUN dotnet build "Habit.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Habit.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Habit.API.dll"]
