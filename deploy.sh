#!/bin/sh
dotnet pack -c release
dotnet nuget push src/bin/Release/ -s https://cognite.jfrog.io/cognite/api/nuget/nuget-local