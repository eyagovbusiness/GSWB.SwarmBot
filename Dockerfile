ARG BUILD_CONFIGURATION=Release ENVIRONMENT=staging
FROM registry.guildswarm.org/$ENVIRONMENT/common:latest AS base-packages

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build 
ARG BUILD_CONFIGURATION
ARG VAULT_ADDR_ENTRYPOINT

WORKDIR /src
# Copy NuGet packages and project files from the base-packages image
COPY --from=base-packages /app/BasePackages ./BasePackages
COPY projectfiles.tar .
# Restore dependencies in a docker cache friendly way
RUN tar -xvf projectfiles.tar \
    && dotnet restore "src/SwarmBot.API/SwarmBot.API.csproj" --configfile ./NuGet.docker.config /p:DockerBuild=true -r linux-musl-x64 \
    && rm projectfiles.tar  # Remove the tar file to reduce image size
COPY . .
RUN dotnet build "src/SwarmBot.API/SwarmBot.API.csproj" -c $BUILD_CONFIGURATION --no-restore /p:DockerBuild=true -r linux-musl-x64

FROM build AS publish
ARG BUILD_CONFIGURATION
RUN dotnet publish "src/SwarmBot.API/SwarmBot.API.csproj" -c $BUILD_CONFIGURATION --no-build -o /app/publish /p:UseAppHost=true /p:DockerBuild=true -r linux-musl-x64

FROM registry.guildswarm.org/baseimages/dotnet/aspnet:8.0 AS final  
WORKDIR /app
COPY --from=publish /app/publish .
COPY Infrastructure/SwarmBotEntrypointOverride.sh ./entrypoint.sh
COPY Infrastructure/ServiceAwait/wait_for_service.sh ./wait_for_service.sh
COPY Infrastructure/ServiceAwait/IsReadyServer.sh ./IsReadyServer.sh
USER root 
RUN chown -R guildswarm:guildswarm /app/ && \
    chmod -R 700 /app/ 
USER guildswarm 
ENTRYPOINT ["/app/entrypoint.sh ${VAULT_ADDR_ENTRYPOINT}"]