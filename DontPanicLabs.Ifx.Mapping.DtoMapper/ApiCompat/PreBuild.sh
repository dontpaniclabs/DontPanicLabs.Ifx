#!/bin/bash
version=$1
echo "$version"

# Split version by dots — bash 3 compatible (macOS ships bash 3; readarray -d requires bash 4)
IFS='.' read -ra versionNumbers <<< "$version"

# No prior version when major is 0 (pre-1.0 build) — skip download
if [[ ${versionNumbers[0]} -eq "0" ]]; then
    echo "Pre-1.0 build — no previous version to compare against, skipping API compat download."
    exit 0
fi

if [[ ${versionNumbers[1]} -eq "0" && ${versionNumbers[2]%%[-+]*} -eq "0" ]]
then
    oldVersion=$(( ${versionNumbers[0]} - 1 ))
else
    oldVersion=${versionNumbers[0]}
fi
oldVersion="$oldVersion.0.0"
echo "$oldVersion"

rm -rf ../LastMajorVersionBinary

curl "https://globalcdn.nuget.org/packages/automapper.$oldVersion.nupkg" \
    --create-dirs -o "../LastMajorVersionBinary/automapper.$oldVersion.nupkg"

unzip -j "../LastMajorVersionBinary/automapper.$oldVersion.nupkg" \
    lib/netstandard2.1/AutoMapper.dll -d ../LastMajorVersionBinary
