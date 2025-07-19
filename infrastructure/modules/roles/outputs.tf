output "cosmos_read_write_role" {
  value       = azurerm_cosmosdb_sql_role_definition.role.id
  description = "cosmos-read-write role id"
}