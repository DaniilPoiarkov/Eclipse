resource "azurerm_cosmosdb_sql_role_assignment" "assignment" {
  role_definition_id  = var.role_definition_id
  scope               = var.scope
  account_name        = var.account_name
  principal_id        = var.principal_id
  resource_group_name = var.resource_group_name
}
