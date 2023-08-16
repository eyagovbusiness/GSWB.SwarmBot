#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Mandril/src/MandrilAPI/MandrilAPI.csproj", "Mandril/src/MandrilAPI/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Application/TheGoodFramework.CA.Application/TGF.CA.Application.Setup.csproj", "TheGoodFramework/src/CleanArchitecture/Application/TheGoodFramework.CA.Application/"]
COPY ["TheGoodFramework/src/Common/TheGoodFramework.Common.Logging/TGF.Common.Logging.csproj", "TheGoodFramework/src/Common/TheGoodFramework.Common.Logging/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Infrastructure/Security/TGF.CA.Infrastructure.Identity/TGF.CA.Infrastructure.Identity.csproj", "TheGoodFramework/src/CleanArchitecture/Infrastructure/Security/TGF.CA.Infrastructure.Identity/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Infrastructure/TGF.CA.Infrastructure.Communication/TGF.CA.Infrastructure.Communication.csproj", "TheGoodFramework/src/CleanArchitecture/Infrastructure/TGF.CA.Infrastructure.Communication/"]
COPY ["TheGoodFramework/src/Common/TheGoodFramework.Common.Serialization/TGF.Common.Serialization.csproj", "TheGoodFramework/src/Common/TheGoodFramework.Common.Serialization/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Infrastructure/Security/TGF.CA.Infrastructure.Secrets/TGF.CA.Infrastructure.Secrets.csproj", "TheGoodFramework/src/CleanArchitecture/Infrastructure/Security/TGF.CA.Infrastructure.Secrets/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Infrastructure/TGF.CA.Infrastructure.Discovery/TGF.CA.Infrastructure.Discovery.csproj", "TheGoodFramework/src/CleanArchitecture/Infrastructure/TGF.CA.Infrastructure.Discovery/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Presentation/TGF.CA.Presentation.Middleware/TGF.CA.Presentation.Middleware.csproj", "TheGoodFramework/src/CleanArchitecture/Presentation/TGF.CA.Presentation.Middleware/"]
COPY ["TheGoodFramework/src/Common/TheGoodFramework.Common.ROP/TGF.Common.ROP.csproj", "TheGoodFramework/src/Common/TheGoodFramework.Common.ROP/"]
COPY ["Mandril/src/MandrilBot/MandrilBot.csproj", "Mandril/src/MandrilBot/"]
COPY ["TheGoodFramework/src/Common/TheGoodFramework.Common.Extensions/TGF.Common.Extensions.csproj", "TheGoodFramework/src/Common/TheGoodFramework.Common.Extensions/"]
COPY ["TheGoodFramework/src/Common/TheGoodFramework.Common.Net/TGF.Common.Net.csproj", "TheGoodFramework/src/Common/TheGoodFramework.Common.Net/"]
RUN dotnet restore "Mandril/src/MandrilAPI/MandrilAPI.csproj"
COPY . .
WORKDIR "/src/Mandril/src/MandrilAPI"
RUN dotnet build "MandrilAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MandrilAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Infrastructure/MicroserviceEntrypointOverrides/MandrilEntrypointOverride.sh ./entrypoint.sh
COPY Infrastructure/ServiceAwait/wait_for_service.sh ./wait_for_service.sh
ENTRYPOINT ["sh", "entrypoint.sh"]