resource "azurerm_resource_group" "rg_alerts" {
  name     = "rg-common-alerts"
  location = var.location

  tags = {
    management = "terraform"
  }
}

locals {
  rg_name = azurerm_resource_group.rg_alerts.name
}

resource "azurerm_monitor_action_group" "monitor_action_group" {
  resource_group_name = local.rg_name
  name                = "common-alerts-ag"
  short_name          = "Notify all"

  email_receiver {
    name          = "Send email"
    email_address = var.email_receiver
  }

  azure_app_push_receiver {
    name          = "Push notification"
    email_address = var.email_receiver
  }

  tags = {
    management = "terraform"
  }
}

resource "azurerm_monitor_smart_detector_alert_rule" "failure_anomalies_alert" {
  resource_group_name = local.rg_name
  name                = "Failure Anomalies"
  severity            = "Sev3"
  detector_type       = "FailureAnomaliesDetector"
  frequency           = "PT1M"
  description         = "Failure Anomalies notifies you of an unusual rise in the rate of failed HTTP requests or dependency calls."
  scope_resource_ids  = var.scope_resource_ids

  action_group {
    ids = [
      azurerm_monitor_action_group.monitor_action_group.id
    ]
  }

  tags = {
    environment = var.environment
    management  = "terraform"
  }

  depends_on = [
    azurerm_monitor_action_group.monitor_action_group
  ]
}
