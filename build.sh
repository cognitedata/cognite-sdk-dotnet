#!/bin/sh
DIRS="src test/unit/fsharp test/unit/csharp test/integration/fsharp test/integration/csharp playground/fsharp playground/csharp"
for dir in $DIRS; do
	rm -rf $dir/bin $dir/obj
done
for dir in $DIRS; do
	dotnet build $dir
done