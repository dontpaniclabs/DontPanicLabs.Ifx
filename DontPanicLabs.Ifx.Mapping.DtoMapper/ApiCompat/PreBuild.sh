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

curl "https://globalcdn.nuget.org/packages/dontpaniclabs.ifx.mapping.dtomapper.$oldVersion.nupkg" \
    --create-dirs -o "../LastMajorVersionBinary/dontpaniclabs.ifx.mapping.dtomapper.$oldVersion.nupkg"

unzip -j "../LastMajorVersionBinary/dontpaniclabs.ifx.mapping.dtomapper.$oldVersion.nupkg" \
    lib/net10.0/DontPanicLabs.Ifx.Mapping.DtoMapper.dll -d ../LastMajorVersionBinary
