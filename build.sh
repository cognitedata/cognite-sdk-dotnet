#!/bin/sh
DIRS="src test/fsharp test/csharp playground/fsharp playground/csharp"
for dir in $DIRS; do
	rm -rf $dir/bin $dir/obj
done
for dir in $DIRS; do
	dotnet build $dir
done