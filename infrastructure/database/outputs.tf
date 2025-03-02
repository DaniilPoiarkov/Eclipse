output "cosmos_endpoint" {
  value = azurerm_cosmosdb_account.cosmosdb.endpoint
}

output "database_name" {
  value = azurerm_cosmosdb_sql_database.database.name
}
