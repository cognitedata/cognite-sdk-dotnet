#!/bin/bash
source ./test_auth.sh
dotnet test CogniteSdk/test/fsharp /p:Exclude="[xunit*]*" /p:Exclude="[CogniteSdk.Protobuf*]*" /p:CollectCoverage=true /p:CoverletOutputFormat=json /p:CoverletOutput='coverage.json' /p:SolutionDir=$(pwd)
dotnet test CogniteSdk/test/csharp /p:Exclude="[xunit*]*" /p:Exclude="[CogniteSdk.Protobuf*]*" /p:CollectCoverage=true /p:MergeWith='../fsharp/coverage.json' /p:CoverletOutputFormat=lcov /p:CoverletOutput='../../../coverage.lcov' /p:SolutionDir=$(pwd)
rm CogniteSdk/test/fsharp/coverage.json
