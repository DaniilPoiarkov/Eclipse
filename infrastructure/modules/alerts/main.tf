resource "azurerm_resource_group" "rg_alerts" {
  name     = "rg-common-alerts"
  location = var.location
  tags     = local.tags
}

locals {
  rg_name     = azurerm_resource_group.rg_alerts.name
  threashold  = 1
  time_window = 15
  frequency   = 15

  tags = {
    management = "terraform"
  }
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

  tags = local.tags
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

  depends_on = [
    azurerm_monitor_action_group.monitor_action_group
  ]

  tags = local.tags
}

resource "azurerm_monitor_scheduled_query_rules_alert" "failed_dependency_calls_query" {
  for_each = var.dependency_targets

  name                = "Failed calls: ${each.key}"
  location            = var.location
  data_source_id      = var.data_source_id
  severity            = var.severity
  resource_group_name = local.rg_name
  frequency           = local.frequency
  time_window         = local.time_window

  query = <<-QUERY
    dependencies
    | where success == false and target == '${each.key}'
    QUERY

  action {
    action_group = [
      azurerm_monitor_action_group.monitor_action_group
    ]
  }

  trigger {
    operator  = "GreaterThanOrEqual"
    threshold = local.threashold
  }

  tags = local.tags
}
