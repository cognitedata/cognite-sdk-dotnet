#!/bin/sh
dotnet test test/fsharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:CoverletOutputFormat=json /p:CoverletOutput='coverage.json'
dotnet test test/csharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:MergeWith='../fsharp/coverage.json' /p:CoverletOutputFormat=lcov /p:CoverletOutput='../../coverage.lcov'
rm test/fsharp/coverage.json
