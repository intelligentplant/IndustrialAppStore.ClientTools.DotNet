#requires -version 5

<#
.SYNOPSIS
Sets TeamCity build parameters.
.DESCRIPTION
When building in TeamCity, this script can be used to set the build number, as well as the AssemblyVersion, AssemblyFileVersion, PackageVersion, and InformationalVersion parameters.
.PARAMETER Branch
The VCS branch name (i.e. "%teamcity.build.branch%").
.PARAMETER BuildCounter
The TeamCity build counter (i.e. %build.counter%).
.PARAMETER BuildMetadata
Build metadata to append to the InformationalVersion property.
.EXAMPLE
Building release packages.
    Set-TeamCityParameters.ps1 -BuildCounter 47 -Branch master
#>
[CmdletBinding(PositionalBinding = $false, DefaultParameterSetName='Groups')]
param(

    [Parameter(Mandatory=$true)]
    [string]
    $BuildCounter,

    [string]
    $BuildMetadata = '',

    [string]
    $Branch = 'master'
)

$Version = . "$PSScriptRoot/Get-Version.ps1"

$BuildNumber = "$($Version.PackageVersion)+${Branch}-${BuildCounter}"
if ([string]::IsNullOrWhiteSpace($BuildMetadata)) {
    $InformationalVersion = $BuildNumber
} else {
    $InformationalVersion = "$($BuildNumber)$($BuildMetadata)"
}

Write-Host "Setting assembly version: $($Version.AssemblyVersion)"
Write-Host "##teamcity[setParameter name='system.AssemblyVersion' value='$($Version.AssemblyVersion)']"

Write-Host "Setting assembly file version: $($Version.FileVersion)"
Write-Host "##teamcity[setParameter name='system.AssemblyFileVersion' value='$($Version.FileVersion)']"

Write-Host "Setting package version: $($Version.PackageVersion)"
Write-Host "##teamcity[setParameter name='system.PackageVersion' value='$($Version.PackageVersion)']"

Write-Host "Setting informational version: ${InformationalVersion}"
Write-Host "##teamcity[setParameter name='system.InformationalVersion' value='${InformationalVersion}']"

Write-Host "Setting build number: ${BuildNumber}"
Write-Host "##teamcity[buildNumber '${BuildNumber}']"
