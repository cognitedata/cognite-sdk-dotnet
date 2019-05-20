#!/bin/sh
cp /nuget-credentials/nuget.config ./nuget.config
TOKEN=`xmllint --xpath 'string(/configuration/apikeys/add/@value)' ./nuget.config`
dotnet pack -c release
dotnet nuget push src/bin/Release/ -k $TOKEN -s https://cognite.jfrog.io/cognite/api/nuget/nuget-local