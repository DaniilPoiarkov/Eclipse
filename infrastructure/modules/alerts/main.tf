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

resource "azurerm_logic_app_workflow" "logic_app" {
  name                = "logic-send-tg-alert"
  location            = var.location
  resource_group_name = local.rg_name
  tags                = local.tags
}

resource "azurerm_logic_app_trigger_http_request" "http_trigger" {
  name         = "When HTTP request is received"
  logic_app_id = azurerm_logic_app_workflow.logic_app.id
  schema       = <<SCHEMA
  {
    "type": "object",
    "properties": {
      "schemaId": {
        "type": "string"
      },
      "data": {
        "type": "object",
        "properties": {
          "essentials": {
            "type": "object",
            "properties": {
              "alertId": {
                "type": "string"
              },
              "alertRule": {
                "type": "string"
              },
              "severity": {
                "type": "string"
              },
              "signalType": {
                "type": "string"
              },
              "monitorCondition": {
                "type": "string"
              },
              "monitoringService": {
                "type": "string"
              },
              "alertTargetIDs": {
                "type": "array",
                "items": {
                  "type": "string"
                }
              },
              "originAlertId": {
                "type": "string"
              },
              "firedDateTime": {
                "type": "string"
              },
              "resolvedDateTime": {
                "type": "string"
              },
              "description": {
                "type": "string"
              },
              "essentialsVersion": {
                "type": "string"
              },
              "alertContextVersion": {
                "type": "string"
              }
            }
          },
          "alertContext": {
            "type": "object",
            "properties": {}
          }
        }
      }
    }
  }
  SCHEMA
}

resource "azurerm_logic_app_action_http" "http_call" {
  name         = "Send telegram alert"
  logic_app_id = azurerm_logic_app_workflow.logic_app.id
  method       = "POST"
  uri          = "https://api.telegram.org/bot${var.tg_allert_bot_token}/sendMessage"
  body         = <<EOT
    { 
      "chat_id": "${var.tg_alerts_chat}",
      "text": "⚠️@{triggerBody()?['data']?['essentials']?['alertRule']}\n🕑Triggered at: @{triggerBody()?['data']?['essentials']?['firedDateTime']}\nMetric value: @{triggerBody()?['data']?['alertContext']?['condition']?['allOf'][0]?['metricValue']}"
    }
    EOT
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

  logic_app_receiver {
    name                    = azurerm_logic_app_workflow.logic_app.name
    resource_id             = azurerm_logic_app_workflow.logic_app.id
    callback_url            = azurerm_logic_app_trigger_http_request.http_trigger.callback_url
    use_common_alert_schema = true
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

  name                = "Failed calls to ${each.key}"
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
      azurerm_monitor_action_group.monitor_action_group.id
    ]
  }

  trigger {
    operator  = "GreaterThanOrEqual"
    threshold = local.threashold
  }

  tags = local.tags
}

resource "azurerm_monitor_scheduled_query_rules_alert" "failed_inbox_messages" {
  name                = "Failed to process inbox messages"
  location            = var.location
  data_source_id      = var.data_source_id
  severity            = var.severity
  resource_group_name = local.rg_name
  frequency           = local.frequency
  time_window         = local.time_window

  query = <<-QUERY
    traces
    | where severityLevel == 3 and message contains 'Failed to process inbox message'
    QUERY

  action {
    action_group = [
      azurerm_monitor_action_group.monitor_action_group.id
    ]
  }

  trigger {
    operator  = "GreaterThanOrEqual"
    threshold = local.threashold
  }

  tags = local.tags
}
