#!/bin/sh
dotnet test CogniteSdk/test/fsharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:CoverletOutputFormat=json /p:CoverletOutput='coverage.json'
dotnet test CogniteSdk/test/csharp /p:Exclude="[xunit*]*" /p:CollectCoverage=true /p:MergeWith='../fsharp/coverage.json' /p:CoverletOutputFormat=lcov /p:CoverletOutput='../../../coverage.lcov'
rm CogniteSdk/test/fsharp/coverage.json
