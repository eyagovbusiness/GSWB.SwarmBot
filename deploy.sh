#!/bin/bash
set -eux

DockerVersion=1.0.0

# tarball csproj files, sln files, and NuGet.config
find . \( -name "*.csproj" -o -name "*.sln" -o -name "NuGet.config" \) -print0 \
    | tar -cvf projectfiles.tar --null -T -

docker build . -t gswb-mandril:$DockerVersion -t gswb-mandril:latest

rm projectfiles.tar