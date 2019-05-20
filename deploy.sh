#!/bin/sh
cp /jenkins-docker-builder/nuget.config ./nuget.config
TOKEN=`xmllint --xpath 'string(/configuration/apikeys/add/@value)' ./nuget.config`
dotnet pack -c release
dotnet nuget push src/bin/Release/CogniteSdk.*.nupkg -k $TOKEN -s https://cognite.jfrog.io/cognite/api/nuget/nuget-local