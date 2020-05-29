#requires -version 5

<#
.SYNOPSIS
Builds this repository.
.DESCRIPTION
This script runs MSBuild on this repository.
.PARAMETER Configuration
Specify MSBuild configuration: Debug, Release
.PARAMETER Clean
Specifies if the "Clean" target should be run prior to the "Build" target.
.PARAMETER NoBuild
When specified, "Build" and "Pack" targets will not be run.
.PARAMETER Pack
Produce NuGet packages.
.PARAMETER Sign
Sign assemblies and NuGet packages (requires additional configuration not provided by this script).
.PARAMETER CI
Sets the MSBuild "ContinuousIntegrationBuild" property to "true".
.PARAMETER Verbosity
MSBuild verbosity: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
.PARAMETER MSBuildArguments
Additional MSBuild arguments to be passed through.
.EXAMPLE
Building release packages.
    build.ps1 -Configuration Release -Pack /p:some_param=some_value
#>
[CmdletBinding(PositionalBinding = $false, DefaultParameterSetName='Groups')]
param(
    [ValidateSet('Debug', 'Release')]
    [string]
    $Configuration,

    [switch]
    $Clean,

    [switch]
    $NoBuild,

    [switch]
    $Pack,

    [switch]
    $Sign,

    [switch]
    $CI,
    
    [string]
    $Verbosity = 'minimal',

    [switch]
    $Help,

    # Remaining arguments will be passed to MSBuild directly
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]
    $MSBuildArguments
)

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'

if ($Help) {
    Get-Help $PSCommandPath -Detailed
    exit 1
}

$SolutionFile = (Get-ChildItem -Path "$PSScriptRoot/*.sln" -Force -File | Select-Object -First 1).Name
$Artifacts = "$PSScriptRoot/artifacts"

. "$PSScriptRoot/build/tools.ps1"

# Set MSBuild verbosity
$MSBuildArguments += "-v:$Verbosity"

# Select targets
$MSBuildTargets = @()
if ($Clean) {
    $MSBuildTargets += 'Clean'
}
if (-Not $NoBuild) {
    $MSBuildTargets += 'Build'
    if ($Pack) {
        $MSBuildTargets += 'Pack'
    }
}

$local:targets = [string]::Join(';',$MSBuildTargets)
$MSBuildArguments += "-t:""$targets"""

if (-Not $NoBuild) {
    # Set default configuration if required
    if (-not $Configuration) {
        $Configuration = 'Debug'
    }
    $MSBuildArguments += "-p:""Configuration=$Configuration"""

    # If the Sign flag is set, add a SignOutput build argument.
    if ($Sign) {
        $MSBuildArguments += "-p:""SignOutput=true"""
    }

    # Configure version numbers to use in build.
    $Version = . "$PSScriptRoot/build/Get-Version.ps1"

    $MSBuildArguments += "/p:""AssemblyVersion=$($Version.AssemblyVersion)"""
    $MSBuildArguments += "/p:""FileVersion=$($Version.FileVersion)"""
    $MSBuildArguments += "/p:""Version=$($Version.PackageVersion)"""
}

if ($CI) {
    $MSBuildArguments += "/p:ContinuousIntegrationBuild=true"
}

$local:exit_code = $null
try {
    # Run the build
    Run-Build
}
catch {
    Write-Host $_.ScriptStackTrace
    $exit_code = 1
}
finally {
    if (! $exit_code) {
        $exit_code = $LASTEXITCODE
    }
}

exit $exit_code