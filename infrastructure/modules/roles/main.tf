resource "azurerm_cosmosdb_sql_role_definition" "role" {
  account_name        = var.account_name
  resource_group_name = var.resource_group_name
  name                = "cosmos-read-write"

  assignable_scopes = var.assignable_scopes

  permissions {
    data_actions = [
      "Microsoft.DocumentDB/databaseAccounts/readMetadata",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*",
      "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*"
    ]
  }
}

resource "azurerm_cosmosdb_sql_role_assignment" "assignment" {
  role_definition_id  = azurerm_cosmosdb_sql_role_definition.role.id
  scope               = var.scope
  account_name        = var.account_name
  principal_id        = var.cosmos_principal
  resource_group_name = var.resource_group_name
}
