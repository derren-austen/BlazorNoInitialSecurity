# API app

$sourceFile = '.\AnApiNotPartOfTheBlazorInfrastructure\appsettings.json'
$destinations = '.\AnApiNotPartOfTheBlazorInfrastructure\appsettings.Development.json'

Foreach ($destination in $destinations)
{
    if (-not (test-path $destination))
    {
        Copy-Item $sourceFile -Destination $destination
    }
}

# WASM host server app

$sourceFile = '.\BlazorHosted\Server\appsettings.json'
$destinations = '.\BlazorHosted\Server\appsettings.Development.json'

Foreach ($destination in $destinations)
{
    if (-not (test-path $destination))
    {
        Copy-Item $sourceFile -Destination $destination
    }
}