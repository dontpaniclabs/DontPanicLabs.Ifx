param([string]$version)
echo $version
$versionNumbers = $version.Split(".")

# No prior version when major is 0 (pre-1.0 build) — skip download
if ($versionNumbers[0] -eq "0") {
    echo "Pre-1.0 build — no previous version to compare against, skipping API compat download."
    exit 0
}

if ($versionNumbers[1] -eq "0" -AND ($versionNumbers[2] -split "[-+]")[0] -eq "0") {
    $oldVersion = [int]$versionNumbers[0] - 1
} else {
    $oldVersion = $versionNumbers[0]
}
$oldVersion = $oldVersion.ToString() + ".0.0"
echo $oldVersion

& ..\..\nuget install DontPanicLabs.Ifx.Mapping.DtoMapper -Version $oldVersion -OutputDirectory ..\LastMajorVersionBinary
& copy "..\LastMajorVersionBinary\DontPanicLabs.Ifx.Mapping.DtoMapper.$oldVersion\lib\net10.0\DontPanicLabs.Ifx.Mapping.DtoMapper.dll" ..\LastMajorVersionBinary
