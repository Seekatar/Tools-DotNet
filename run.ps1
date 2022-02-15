param (
    [ArgumentCompleter({
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameters)
        $runFile = (Join-Path (Split-Path $commandAst -Parent) run.ps1)
        if (Test-Path $runFile) {
            Get-Content $runFile |
                    Where-Object { $_ -match "^\s+'([\w+-]+)' {" } |
                    ForEach-Object {
                        if ( !($fakeBoundParameters[$parameterName]) -or
                            (($matches[1] -notin $fakeBoundParameters.$parameterName) -and
                             ($matches[1] -like "*$wordToComplete*"))
                            )
                        {
                            $matches[1]
                        }
                    }
        }
     })]
    [string[]] $Tasks,
    [string] $Version,
    [string] $LocalNugetFolder
)

function executeSB
{
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [scriptblock] $ScriptBlock,
    [string] $WorkingDirectory
)
    Set-StrictMode -Version Latest

    if ($WorkingDirectory) {
        Push-Location $WorkingDirectory
    }
    try {
        Invoke-Command -ScriptBlock $ScriptBlock

        if ($LASTEXITCODE -ne 0) {
            throw "Error executing command, last exit $LASTEXITCODE"
        }
    } finally {
        if ($WorkingDirectory) {
            Pop-Location
        }
    }
}

if ($Tasks -eq "ci") {
    $myTasks = @('CreateLocalNuget','ObjectFactoryBuild','ObjectFactoryTest','ObjectFactoryPack')
} else {
    $myTasks = $Tasks
}

foreach ($t in $myTasks) {

    try {

        $prevPref = $ErrorActionPreference
        $ErrorActionPreference = "Stop"

        "-------------------------------"
        "Starting $t"
        "-------------------------------"

        switch ($t) {
            'CreateLocalNuget' {
                executeSB -WorkingDirectory $PSScriptRoot {
                    $localNuget = dotnet nuget list source | Select-String "Local \[Enabled" -Context 0,1
                    if (!$localNuget) {
                        if (!$LocalNugetFolder) {
                            $LocalNugetFolder = (Join-Path $PSScriptRoot 'packages')
                            $null = New-Item 'packages' -ItemType Directory -ErrorAction Ignore
                        }
                        dotnet nuget add source $LocalNugetFolder --name Local
                    }
                    }
            }
            'Build' {
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/src/Tools') {
                    dotnet build
                    }
            }
            'Test' {
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/tests/unit') {
                    dotnet build
                    }
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/tests/unit') {
                    $localNuget = dotnet nuget list source | Select-String "Local \[Enabled" -Context 0,1
                    if ($localNuget) {
                        dotnet pack -o ($localNuget.Context.PostContext.Trim()) --include-source -p:Version=1.0.2 -p:AssemblyVersion=1.0.2
                    } else {
                        throw "Must have a Local NuGet source for testing. e.g. dotnet nuget sources add -name Local -source c:\nupkgs"
                    }
                    }
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/tests/unit') {
                    dotnet test --collect:"XPlat Code Coverage"
                    }
            }
            'Pack' {
                if ($Version) {
                    "Packing with version $Version"
                    executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/src/Tools') {
                        dotnet pack -o ../../packages --include-source -p:Version=$Version -p:AssemblyVersion=$Version
                    }
                } else {
                    throw "Must supply Version for pack"
                }
            }
        }

    } finally {
        $ErrorActionPreference = $prevPref
    }
}