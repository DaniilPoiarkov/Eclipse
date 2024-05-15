Import-Module "$env:ProgramFiles\Azure Cosmos DB Emulator\PSModules\Microsoft.Azure.CosmosDB.Emulator"

Microsoft.Azure.Cosmos.Emulator.exe /GenKeyFile=eclipsecosmosdbauthkey

CosmosDB.Emulator.exe /AllowNetworkAccess /KeyFile=eclipsecosmosdbauthkey

# Start-CosmosDbEmulator -AllowNetworkAccess
