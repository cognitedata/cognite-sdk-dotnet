#!/bin/sh
DIRS="CogniteSdk.Types Oryx.Cognite CogniteSdk/src CogniteSdk/test/csharp CogniteSdk/test/csharp playground/csharp playground/fsharp"
for dir in $DIRS; do
	rm -rf $dir/bin $dir/obj
done
for dir in $DIRS; do
	dotnet build $dir
done