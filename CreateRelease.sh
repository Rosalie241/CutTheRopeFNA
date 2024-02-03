#!/usr/bin/env bash
set -e

targets=(
	"linux-x64:Linux-x64"
	"linux-arm64:Linux-arm64"
	"win-x64:Windows-x64"
	"win-x86:Windows-x86"
)

revision=$(git describe --tags --always)

source_dir="$(realpath $(dirname "$0"))"

echo "-> Cleaning..."
dotnet clean > /dev/null

for target in ${targets[@]}
do
	rid=${target%%:*}
	target_name=${target#*:}
	echo "-> Building $rid..."
	dotnet publish -r $rid -c Release --self-contained /p:DebugType=None /p:DebugSymbols=false
	echo "-> Packaging $rid..."
	pushd "$source_dir/bin/Release/net8.0/$rid/publish" > /dev/null
	7za a -mm=Deflate -mx=9 -r "${source_dir}/bin/CutTheRope-${target_name}-${revision}.zip" *
	popd > /dev/null
done
