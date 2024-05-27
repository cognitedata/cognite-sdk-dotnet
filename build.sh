#!/bin/sh
DIRS="CogniteSdk.Types Oryx.Cognite/src CogniteSdk/src CogniteSdk.FSharp/src CogniteSdk/test/fsharp CogniteSdk/test/csharp Examples/csharp Examples/fsharp"
for dir in $DIRS; do
	rm -rf $dir/bin $dir/obj
done
for dir in $DIRS; do
	dotnet build $dir
done