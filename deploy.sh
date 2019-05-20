#!/bin/sh
cp /jenkins-docker-builder/Nuget.Config .
dotnet pack -c release
dotnet nuget push src/bin/Release/CogniteSdk.*.nupkg -s https://cognite.jfrog.io/cognite/api/nuget/nuget-local