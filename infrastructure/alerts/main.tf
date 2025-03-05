resource "azurerm_log_analytics_workspace" "log_analytics_workspace" {
  location            = var.location
  resource_group_name = var.resource_group_name
  name                = "${var.app_name}-${var.environment}-log"

  tags = {
    environment = var.environment
    source      = "terraform"
  }
}

resource "azurerm_monitor_action_group" "monitor_action_group" {
  resource_group_name = var.resource_group_name
  name                = "${var.app_name}-${var.environment}-ag"
  short_name          = "${var.app_name}-ag"

  tags = {
    environment = var.environment
    source      = "terraform"
  }
}

resource "azurerm_application_insights" "app_insights" {
  workspace_id        = azurerm_log_analytics_workspace.log_analytics_workspace.id
  name                = "${var.app_name}-${var.environment}-appi"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = var.application_type

  tags = {
    environment = var.environment
    source      = "terraform"
  }

  depends_on = [
    azurerm_log_analytics_workspace.log_analytics_workspace
  ]
}

resource "azurerm_monitor_smart_detector_alert_rule" "failure_anomalies_alert" {
  resource_group_name = var.resource_group_name
  name                = "Failure Anomalies"
  severity            = "Sev3"
  detector_type       = "FailureAnomaliesDetector"
  frequency           = "PT1M"
  description         = "Failure Anomalies notifies you of an unusual rise in the rate of failed HTTP requests or dependency calls."

  scope_resource_ids = [
    azurerm_application_insights.app_insights.id
  ]

  action_group {
    ids = [
      azurerm_monitor_action_group.monitor_action_group.id
    ]
  }

  tags = {
    environment                              = var.environment
    source                                   = "terraform"
    "hidden-link: /app-insights-resource-id" = azurerm_application_insights.app_insights.id
  }

  depends_on = [
    azurerm_application_insights.app_insights,
    azurerm_monitor_action_group.monitor_action_group
  ]
}
