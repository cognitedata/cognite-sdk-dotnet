#!/bin/sh
dotnet test test/unit/fsharp /p:Exclude="[xunit*]*%2c[proto*]*" /p:CollectCoverage=true /p:CoverletOutputFormat=json /p:CoverletOutput='coverage.json'
dotnet test test/unit/csharp /p:Exclude="[xunit*]*%2c[proto*]*" /p:CollectCoverage=true /p:MergeWith='../fsharp/coverage.json' /p:CoverletOutputFormat=json /p:CoverletOutput='coverage.json'
dotnet test test/integration /p:Exclude="[xunit*]*%2c[proto*]*" /p:CollectCoverage=true /p:MergeWith='../unit/csharp/coverage.json' /p:CoverletOutputFormat=lcov /p:CoverletOutput='../../coverage.lcov'
rm test/unit/fsharp/coverage.json
rm test/unit/csharp/coverage.json
