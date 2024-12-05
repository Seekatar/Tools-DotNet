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
    [string] $LocalNugetFolder,
    [string] $NugetVersion
)

function executeSB
{
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [scriptblock] $ScriptBlock,
    [string] $WorkingDirectory,
    [string] $Name
)
    Set-StrictMode -Version Latest

    if ($WorkingDirectory) {
        Push-Location $WorkingDirectory
    }
    try {
        Invoke-Command -ScriptBlock $ScriptBlock

        if ($LASTEXITCODE -ne 0) {
            throw "Error executing command '$Name', last exit $LASTEXITCODE"
        }
    } finally {
        if ($WorkingDirectory) {
            Pop-Location
        }
    }
}

if ($Tasks -eq "ci") {
    $myTasks = @('CreateLocalNuget','Build','Test','Pack')
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
                executeSB -WorkingDirectory $PSScriptRoot -Name 'CreateNuget' {
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
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/src/Tools') -Name 'Build' {
                    dotnet build
                    }
            }
            'Test' {
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/tests/ObjectFactoryTests/ObjectFactoryTestWorkers') -Name 'Build test worker' {
                    $localNuget = dotnet nuget list source | Select-String "Local \[Enabled" -Context 0,1
                    if ($localNuget) {
                        # pack directly to local nuget folder since on build box, can't push to local
                        dotnet pack -o ($localNuget.Context.PostContext.Trim()) --include-source -p:Version=1.0.2 -p:AssemblyVersion=1.0.2
                    } else {
                        throw "Must have a Local NuGet source for testing. e.g. dotnet nuget sources add -name Local -source c:\nupkgs"
                    }
                    }
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/tests/unit') -Name 'Build test' {
                    dotnet build
                    }
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/tests/unit') -Name 'Run test' {
                    dotnet test --collect:"XPlat Code Coverage"
                    }
            }
            'Pack' {
                if ($Version) {
                    "Packing with version $Version in $PWD"
                    executeSB -WorkingDirectory (Join-Path $PSScriptRoot '/src/Tools') -Name 'Pack' {
                        $justVersion = $Version.Split('-')[0]
                        dotnet pack -o ../../packages --include-source -p:Version=$Version -p:PackageVersion=$Version -p:AssemblyVersion=$justVersion
                    }
                } else {
                    throw "Must supply Version for pack"
                }
            }
            'RunMinimal' {
                if ($NugetVersion) {
                    $env:NUGET_VERSION = $NugetVersion
                }
                executeSB -WorkingDirectory (Join-Path $PSScriptRoot 'tests\WebSettingsMinimalApi') -Name 'RunMinimal' {
                    dotnet run
                }
                if ($NugetVersion) {
                    $env:NUGET_VERSION = $null
                }
            }
            default {
                throw "Invalid task name $t"
            }
        }

    } finally {
        $ErrorActionPreference = $prevPref
    }
}