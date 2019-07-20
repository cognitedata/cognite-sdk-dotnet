#!/bin/sh
dotnet test test/unit/fsharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:CoverletOutputFormat=json /p:CoverletOutput='coverage.json'
dotnet test test/unit/csharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:MergeWith='../fsharp/coverage.json' /p:CoverletOutputFormat=lcov /p:CoverletOutput='../../../coverage.lcov'
rm test/unit/fsharp/coverage.json
