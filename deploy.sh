#!/bin/sh
dotnet pack -c release -p:PackageVersion=$TRAVIS_TAG
dotnet nuget push CogniteSdk.Types/bin/Release/*.nupkg -s $NUGET_SOURCE -k $NUGET_API_KEY
dotnet nuget push CogniteSdk/src/bin/Release/*.nupkg -s $NUGET_SOURCE -k $NUGET_API_KEY
dotnet nuget push Oryx.Cognite/src/bin/Release/*.nupkg -s $NUGET_SOURCE -k $NUGET_API_KEY
