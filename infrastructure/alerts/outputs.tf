output "app_insights_id" {
  value     = azurerm_application_insights.app_insights.id
  sensitive = true
}

output "app_insights_connection_string" {
  value     = azurerm_application_insights.app_insights.connection_string
  sensitive = true
}
