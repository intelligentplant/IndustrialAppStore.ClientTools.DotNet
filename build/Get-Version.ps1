#requires -version 5

<#
.SYNOPSIS
Gets the project versions to use when building MSBuild projects.
.DESCRIPTION
Returns an object that defines the following version properties: AssemblyVersion, FileVersion, PackageVersion.
.PARAMETER RevisionVersion
The revision number for the build (e.g. the build counter from a CI system).
#>
[CmdletBinding(PositionalBinding = $false, DefaultParameterSetName='Groups')]
param(

    [int]
    [ValidateRange(0, [int]::MaxValue)]
    $RevisionVersion = 0

)

$Version = Get-Content "$PSScriptRoot/version.json" | Out-String | ConvertFrom-Json

$MajorMinorVersion = "$($Version.Major).$($Version.Minor)"
$MajorMinorPatchVersion = "${MajorMinorVersion}.$($Version.Patch)"

$AssemblyVersion = "${MajorMinorVersion}.0.0"
$FileVersion = "${MajorMinorPatchVersion}.${RevisionVersion}"
if ([string]::IsNullOrWhiteSpace($version.PreRelease)) {
    $PackageVersion = $MajorMinorPatchVersion
} else {
    if ($RevisionVersion -gt 0) {
        $PackageVersion = "${MajorMinorPatchVersion}-$($version.PreRelease).${RevisionVersion}"
    } else {
        $PackageVersion = "${MajorMinorPatchVersion}-$($version.PreRelease)"
    }
}

$result = @{}
$result.AssemblyVersion = $AssemblyVersion
$result.FileVersion = $FileVersion
$result.PackageVersion = $PackageVersion

return $result
