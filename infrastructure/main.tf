resource "azurerm_resource_group" "resource_group_main" {
  location = var.location
  name     = "rg-${var.app_name}-${var.environment}"

  tags = {
    environment = var.environment
    source      = "terraform"
  }
}

locals {
  resource_group_name = azurerm_resource_group.resource_group_main.name
  resource_group_id   = azurerm_resource_group.resource_group_main.id
  location            = azurerm_resource_group.resource_group_main.location
}

module "entraid" {
  source = "./entraid"
  app_name = var.app_name
  environment = var.environment
  spa_redirect_uris = [ "https://localhost:7236/swagger/oauth2-redirect.html" ]
}

# module "alerts" {
#   source              = "./alerts"
#   resource_group_name = local.resource_group_name
#   location            = local.location
#   environment         = var.environment
#   app_name            = var.app_name
#   application_type    = "web"
#   email_receiver      = var.email_receiver

#   depends_on = [
#     azurerm_resource_group.resource_group_main
#   ]
# }

# module "database" {
#   source               = "./database"
#   resource_group_name  = local.resource_group_name
#   location             = local.location
#   cosmos_consistency   = var.cosmos_consistency
#   cosmos_kind          = var.cosmos_kind
#   environment          = var.environment
#   app_name             = var.app_name
#   cosmos_free_tier     = var.cosmos_free_tier
#   cosmos_offer_type    = var.cosmos_offer_type
#   container_name       = var.cosmos_container
#   partition_paths      = var.partiton_paths
#   database_throughput  = var.database_throughput
#   container_throughput = var.container_throughput
#   failover_priority    = var.failover_priority

#   depends_on = [
#     azurerm_resource_group.resource_group_main
#   ]
# }

# module "web-app" {
#   source                         = "./webapp"
#   resource_group_name            = local.resource_group_name
#   location                       = local.location
#   environment                    = var.environment
#   app_name                       = var.app_name
#   app_insights_connection_string = module.alerts.app_insights_connection_string
#   secret_token                   = var.secret_token
#   service_plan_sku_name          = var.service_plan_sku_name
#   settings_use_redis             = var.settings_use_redis
#   sheets_id                      = var.sheets_id
#   sheets_range                   = var.sheets_range
#   chat_id                        = var.chat_id
#   docker_url                     = var.docker_url
#   docker_password                = var.docker_password
#   docker_username                = var.docker_username
#   authorization_key              = var.authorization_key
#   bot_token                      = var.bot_token
#   cosmos_database_container      = var.cosmos_container
#   cosmos_database_id             = module.database.database_name
#   cosmos_endpoint                = module.database.cosmos_endpoint
#   image_name                     = var.image_name
#   google_credentials             = file(var.google_credentials_path)

#   depends_on = [
#     azurerm_resource_group.resource_group_main,
#     module.alerts,
#     module.database
#   ]
# }

# module "roles" {
#   source              = "./roles"
#   resource_group_name = local.resource_group_name
#   cosmos_principal    = module.web-app.app_principal_id
#   account_name        = module.database.account_name
#   scope               = module.database.account_id

#   assignable_scopes = [
#     module.database.account_id
#   ]

#   depends_on = [
#     module.database,
#     module.web-app
#   ]
# }
