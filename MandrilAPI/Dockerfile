#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["MandrilAPI/MandrilAPI.csproj", "MandrilAPI/"]
COPY ["MandrilBot/MandrilBot.csproj", "MandrilBot/"]
COPY ["TheGoodFramework/TheGoodFramework.CA.Domain/TheGoodFramework.CA.Domain.csproj", "TheGoodFramework/TheGoodFramework.CA.Domain/"]
COPY ["TheGoodFramework/TheGoodFramework.Extensions/TheGoodFramework.Extensions.csproj", "TheGoodFramework/TheGoodFramework.Extensions/"]
RUN dotnet restore "MandrilAPI/MandrilAPI.csproj"
COPY . .
WORKDIR "/src/MandrilAPI"
RUN dotnet build "MandrilAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MandrilAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MandrilAPI.dll"]