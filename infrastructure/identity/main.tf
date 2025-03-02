resource "azurerm_role_definition" "cosmosdb_read_write" {
  name        = "cosmosdb-read-write"
  scope       = var.resource_group_id
  description = "Allows to perform read and write operations to cosmosdb account"

  permissions {
    actions = [
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/write",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/read",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/delete",
      "Microsoft.DocumentDB/databaseAccounts/services/delete",
      "Microsoft.DocumentDB/databaseAccounts/services/write",
      "Microsoft.DocumentDB/databaseAccounts/services/read",
      "Microsoft.DocumentDB/databaseAccounts/listKeys/action",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/partitionMerge/action"
    ]
  }
}

resource "azurerm_user_assigned_identity" "cosmos_identity" {
  resource_group_name = var.resource_group_name
  location            = var.location
  name                = "id-${var.app_name}-${var.environment}-${var.location}"

  tags = {
    environment = var.environment
    app         = var.app_name
    source      = "terraform"
  }
}

resource "azurerm_role_assignment" "cosmos_assignment" {
  role_definition_id = azurerm_role_definition.cosmosdb_read_write.role_definition_resource_id
  principal_id       = azurerm_user_assigned_identity.cosmos_identity.principal_id
  scope              = var.resource_group_id

  depends_on = [
    azurerm_role_definition.cosmosdb_read_write,
    azurerm_user_assigned_identity.cosmos_identity
  ]
}
