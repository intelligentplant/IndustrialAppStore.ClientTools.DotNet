<#
  .SYNOPSIS
  Ensures that a templated project file contains the correct package versions.

  .DESCRIPTION
  This repository uses NuGet Central Package Management, meaning that package versions are defined in a central file instead of in individual project files. This script ensures that package versions from the central file are inserted into the project file templates that form the output of this project.

  .PARAMETER ProjectFile
  The templated project file to update.

  .PARAMETER PackageVersionsFile
  The MSBuild props file containing the central package versions.

  .PARAMETER DefaultPackageVersion
  The default version to use if a referenced package is not found in the central package versions file. This should only be the case for packages that are built by projects in this repository.

  .INPUTS
  None. You can't pipe objects to Insert-PackageVersions.ps1.

  .OUTPUTS
  None. Insert-PackageVersions.ps1 doesn't generate any output.
#>

param(
    [string]$ProjectFile,
    [string]$PackageVersionsFile,
    [string]$DefaultPackageVersion
)

$projectFileContent = Get-Content $ProjectFile -Raw

$packageVersionsXml = [xml](Get-Content $PackageVersionsFile)

$packageVersionReplacer = {
    param($match)

    $packageId = $match.Groups['PackageId'].Value
    $packageVersionNode = $packageVersionsXml.SelectSingleNode("//PackageVersion[@Include=""$packageId""]")
    if (($packageVersionNode -eq $null) -or ($packageVersionNode.Attributes['Version'] -eq $null) -or ([string]::IsNullOrWhiteSpace($packageVersionNode.Attributes['Version'].Value))) {
        $packageVersion = $DefaultPackageVersion
    } else {
        $packageVersion = $packageVersionNode.Attributes['Version'].Value
    }
    return "<PackageReference Include=""$packageId"" Version=""$packageVersion"" />"
}

$content = ([regex]::Replace($projectFileContent, '<PackageReference\s+Include="(?<PackageId>[^"]+)".*?/>', $packageVersionReplacer))
Set-Content -Path $ProjectFile -Value $content
