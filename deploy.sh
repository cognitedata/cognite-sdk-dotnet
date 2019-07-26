#!/bin/sh
mono /usr/local/bin/nuget.exe pack src/Fusion.NET.fsproj -IncludeReferencedProjects
dotnet nuget push *.nupkg -s https://cognite.jfrog.io/cognite/api/nuget/nuget-local