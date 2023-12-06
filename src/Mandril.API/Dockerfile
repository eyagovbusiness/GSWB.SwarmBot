#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Mandril/src/Mandril.API/Mandril.API.csproj", "Mandril/src/Mandril.API/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Presentation/TGF.CA.Presentation/TGF.CA.Presentation.csproj", "TheGoodFramework/src/CleanArchitecture/Presentation/TGF.CA.Presentation/"]
COPY ["TheGoodFramework/src/Common/TGF.Common.Logging/TGF.Common.Logging.csproj", "TheGoodFramework/src/Common/TGF.Common.Logging/"]
COPY ["TheGoodFramework/src/Common/TGF.Common.ROP/TGF.Common.ROP.csproj", "TheGoodFramework/src/Common/TGF.Common.ROP/"]
COPY ["TheGoodFramework/src/Common/TGF.Common.Serialization/TGF.Common.Serialization.csproj", "TheGoodFramework/src/Common/TGF.Common.Serialization/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Infrastructure/TGF.CA.Infrastructure/TGF.CA.Infrastructure.csproj", "TheGoodFramework/src/CleanArchitecture/Infrastructure/TGF.CA.Infrastructure/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Application/TGF.CA.Application/TGF.CA.Application.csproj", "TheGoodFramework/src/CleanArchitecture/Application/TGF.CA.Application/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Domain/TGF.CA.Domain/TGF.CA.Domain.csproj", "TheGoodFramework/src/CleanArchitecture/Domain/TGF.CA.Domain/"]
COPY ["Mandril/src/Mandril.Infrastructure/Mandril.Infrastructure.csproj", "Mandril/src/Mandril.Infrastructure/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Infrastructure/Security/TGF.CA.Infrastructure.Secrets/TGF.CA.Infrastructure.Secrets.csproj", "TheGoodFramework/src/CleanArchitecture/Infrastructure/Security/TGF.CA.Infrastructure.Secrets/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Infrastructure/TGF.CA.Infrastructure.Discovery/TGF.CA.Infrastructure.Discovery.csproj", "TheGoodFramework/src/CleanArchitecture/Infrastructure/TGF.CA.Infrastructure.Discovery/"]
COPY ["Mandril/src/Mandril.Application/Mandril.Application.csproj", "Mandril/src/Mandril.Application/"]
COPY ["Mandril/src/Mandril.Domain/Mandril.Domain.csproj", "Mandril/src/Mandril.Domain/"]
COPY ["Mandril/src/MandrilBot/MandrilBot.csproj", "Mandril/src/MandrilBot/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Infrastructure/Communication/TGF.CA.Infrastructure.Communication.RabbitMQ/TGF.CA.Infrastructure.Communication.RabbitMQ.csproj", "TheGoodFramework/src/CleanArchitecture/Infrastructure/Communication/TGF.CA.Infrastructure.Communication.RabbitMQ/"]
COPY ["TheGoodFramework/src/CleanArchitecture/Infrastructure/Communication/TGF.CA.Infrastructure.Communication/TGF.CA.Infrastructure.Communication.csproj", "TheGoodFramework/src/CleanArchitecture/Infrastructure/Communication/TGF.CA.Infrastructure.Communication/"]
COPY ["TheGoodFramework/src/Common/TGF.Common.Extensions/TGF.Common.Extensions.csproj", "TheGoodFramework/src/Common/TGF.Common.Extensions/"]
COPY ["TheGoodFramework/src/Common/TGF.Common.Net/TGF.Common.Net.csproj", "TheGoodFramework/src/Common/TGF.Common.Net/"]
RUN dotnet restore "Mandril/src/Mandril.API/Mandril.API.csproj"
COPY . .
WORKDIR "/src/Mandril/src/Mandril.API"
RUN dotnet build "Mandril.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mandril.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Infrastructure/MicroserviceEntrypointOverrides/MandrilEntrypointOverride.sh ./entrypoint.sh
COPY Infrastructure/ServiceAwait/wait_for_service.sh ./wait_for_service.sh
COPY Infrastructure/ServiceAwait/IsReadyServer.sh ./IsReadyServer.sh
RUN chmod +x ./IsReadyServer.sh
ENTRYPOINT ["sh", "entrypoint.sh"]