resource "azurerm_cosmosdb_account" "cosmosdb" {
  name                = "cosmos-${var.app_name}-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  kind                = var.cosmos_kind
  offer_type          = var.cosmos_offer_type
  free_tier_enabled   = var.cosmos_free_tier

  geo_location {
    location          = var.location
    failover_priority = var.failover_priority
  }

  consistency_policy {
    consistency_level = var.cosmos_consistency
  }

  tags = {
    environment = var.environment
    source      = "terraform"
  }
}

resource "azurerm_cosmosdb_sql_database" "database" {
  name                = var.app_name
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.cosmosdb.name
  throughput          = var.database_throughput
}

resource "azurerm_cosmosdb_sql_container" "container" {
  name                = var.container_name
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.cosmosdb.name
  database_name       = azurerm_cosmosdb_sql_database.database.name
  partition_key_paths = var.partition_paths
  throughput          = var.container_throughput
}
