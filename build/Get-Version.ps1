#requires -version 5

<#
.SYNOPSIS
Gets the project versions to use when building MSBuild projects.
.DESCRIPTION
Returns an object that defines the following version properties: AssemblyVersion, FileVersion, PackageVersion.
#>

$Version = Get-Content "$PSScriptRoot/version.json" | Out-String | ConvertFrom-Json

$MajorMinorVersion = "$($Version.Major).$($Version.Minor)"
$MajorMinorPatchVersion = "$($MajorMinorVersion).$($Version.Patch)"

$AssemblyVersion = "$($MajorMinorVersion).0.0"
$FileVersion = "$($MajorMinorPatchVersion).0"
if ([string]::IsNullOrWhiteSpace($version.PreRelease)) {
    $PackageVersion = $MajorMinorPatchVersion
} else {
    $PackageVersion = "$($MajorMinorPatchVersion)-$($version.PreRelease)"
}

$result = @{}
$result.AssemblyVersion = $AssemblyVersion
$result.FileVersion = $FileVersion
$result.PackageVersion = $PackageVersion

return $result
