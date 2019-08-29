#!/bin/sh
dotnet test test/unit/fsharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:CoverletOutputFormat=json /p:CoverletOutput='coverage.json'
dotnet test test/unit/csharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:MergeWith='../fsharp/coverage.json' /p:CoverletOutputFormat=json /p:CoverletOutput='coverage.json'
dotnet test test/integration/fsharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:MergeWith='../../unit/fsharp/coverage.json' /p:CoverletOutputFormat=json /p:CoverletOutput='coverage.json'
dotnet test test/integration/csharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:MergeWith='../csharp/coverage.json' /p:CoverletOutputFormat=lcov /p:CoverletOutput='../../../coverage.lcov'
rm test/unit/fsharp/coverage.json
rm test/unit/csharp/coverage.json
rm test/integration/csharp/coverage.json