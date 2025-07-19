locals {
  tags = {
    environment = var.environment
    management  = "terraform"
  }
}

resource "azurerm_log_analytics_workspace" "log_analytics_workspace" {
  location            = var.location
  resource_group_name = var.resource_group_name
  name                = "${var.app_name}-${var.environment}-log"
  tags                = local.tags
}

resource "azurerm_application_insights" "app_insights" {
  workspace_id        = azurerm_log_analytics_workspace.log_analytics_workspace.id
  name                = "${var.app_name}-${var.environment}-appi"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = var.application_type
  tags                = local.tags

  depends_on = [
    azurerm_log_analytics_workspace.log_analytics_workspace
  ]
}
