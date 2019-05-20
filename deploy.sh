#!/bin/sh
#TOKEN=`xmllint --xpath 'string(/configuration/apikeys/add/@value)' ./nuget.config`
dotnet pack -c release
#dotnet nuget push src/bin/Release/ -s https://cognite.jfrog.io/cognite/api/nuget/nuget-local -k $TOKEN
dotnet nuget push src/bin/Release/ -s https://cognite.jfrog.io/cognite/api/nuget/v3/nuget-local