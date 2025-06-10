data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "resource_group_main" {
  location = var.location
  name     = "rg-${var.app_name}"

  tags = {
    environment = var.environment
    source      = "terraform"
  }
}

locals {
  user_object_id      = data.azurerm_client_config.current.object_id
  tenant_id           = data.azurerm_client_config.current.tenant_id
  resource_group_name = azurerm_resource_group.resource_group_main.name
  resource_group_id   = azurerm_resource_group.resource_group_main.id
  location            = azurerm_resource_group.resource_group_main.location
}

module "monitoring" {
  source              = "./modules/monitoring"
  resource_group_name = local.resource_group_name
  location            = local.location
  environment         = var.environment
  application_type    = "web"
  app_name            = var.app_name

  depends_on = [
    azurerm_resource_group.resource_group_main
  ]
}

module "alerts" {
  source         = "./modules/alerts"
  data_source_id = module.monitoring.app_insights_id
  location       = local.location
  environment    = var.environment
  email_receiver = var.email_receiver
  severity       = var.alert_severity

  scope_resource_ids = [
    module.monitoring.app_insights_id
  ]

  dependency_targets = [
    "api.telegram.org"
  ]

  depends_on = [
    azurerm_resource_group.resource_group_main,
    module.monitoring
  ]
}

module "database" {
  source                = "./modules/database"
  resource_group_name   = local.resource_group_name
  location              = local.location
  cosmos_consistency    = var.cosmos_consistency
  cosmos_kind           = var.cosmos_kind
  environment           = var.environment
  app_name              = var.app_name
  cosmos_free_tier      = var.cosmos_free_tier
  cosmos_offer_type     = var.cosmos_offer_type
  container_name        = var.cosmos_container
  partition_paths       = var.partiton_paths
  partition_key_version = var.partition_key_version
  database_throughput   = var.database_throughput
  container_throughput  = var.container_throughput
  failover_priority     = var.failover_priority

  depends_on = [
    azurerm_resource_group.resource_group_main
  ]
}

module "entraid" {
  source          = "./modules/entraid"
  app_name        = var.app_name
  environment     = var.environment
  owner_object_id = local.user_object_id
}

module "web-app" {
  source                         = "./modules/webapp"
  resource_group_name            = local.resource_group_name
  location                       = local.location
  environment                    = var.environment
  app_name                       = var.app_name
  app_insights_connection_string = module.monitoring.app_insights_connection_string
  secret_token                   = var.secret_token
  service_plan_sku_name          = var.service_plan_sku_name
  settings_use_redis             = var.settings_use_redis
  sheets_id                      = var.sheets_id
  sheets_range                   = var.sheets_range
  chat_id                        = var.chat_id
  docker_url                     = var.docker_url
  docker_password                = var.docker_password
  docker_username                = var.docker_username
  authorization_key              = var.authorization_key
  bot_token                      = var.bot_token
  cosmos_database_container      = var.cosmos_container
  cosmos_database_id             = module.database.database_name
  cosmos_endpoint                = module.database.cosmos_endpoint
  image_name                     = var.image_name
  google_credentials             = file(var.google_credentials_path)
  azuread_client_id              = module.entraid.app_client_id
  tenant_id                      = local.tenant_id

  depends_on = [
    azurerm_resource_group.resource_group_main,
    module.alerts,
    module.database,
    module.entraid
  ]
}

module "roles" {
  source              = "./modules/roles"
  resource_group_name = local.resource_group_name
  cosmos_principal    = module.web-app.app_principal_id
  account_name        = module.database.account_name
  scope               = module.database.account_id

  assignable_scopes = [
    module.database.account_id
  ]

  depends_on = [
    module.database,
    module.web-app
  ]
}

module "redirection" {
  source              = "./modules/redirection"
  app_registration_id = module.entraid.app_registration_id

  spa_redirect_uris = [
    "https://${module.web-app.app_hostname}/swagger/oauth2-redirect.html"
  ]

  depends_on = [
    module.entraid,
    module.web-app
  ]
}
