param(
    [Parameter(Mandatory = $true)]
    [string]$ApiKey
)

$ErrorActionPreference = "Stop"

$projectPath = "$PSScriptRoot\..\mcp\Eclipse.MCP\Eclipse.MCP.csproj"
$outputDir   = "$PSScriptRoot\..\mcp\Eclipse.MCP\nupkg"

Write-Host "Packing Eclipse.MCP..."
dotnet pack $projectPath -c Release -o $outputDir

if ($LASTEXITCODE -ne 0) { 
    exit $LASTEXITCODE 
}

$packages = Get-ChildItem $outputDir -Filter "*.nupkg"

if (-not $packages) {
    Write-Error "No .nupkg files found in $outputDir"
    exit 1
}

foreach ($package in $packages) {
    Write-Host "Publishing $($package.Name)..."
    dotnet nuget push $package.FullName --api-key $ApiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
    
    if ($LASTEXITCODE -ne 0) { 
        exit $LASTEXITCODE 
    }
}

Write-Host "Done. Published $($packages.Count) package(s)."
