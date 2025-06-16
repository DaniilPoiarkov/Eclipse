resource "azurerm_service_plan" "service_plan_api" {
  location            = var.location
  name                = "asp-${var.app_name}-${var.environment}"
  os_type             = var.os_type
  resource_group_name = var.resource_group_name
  sku_name            = var.service_plan_sku_name

  tags = {
    environment = var.environment
    management  = "terraform"
  }
}

resource "azurerm_linux_web_app" "app" {
  service_plan_id     = azurerm_service_plan.service_plan_api.id
  resource_group_name = var.resource_group_name
  name                = "${var.app_name}-${var.environment}-app"
  location            = var.location

  site_config {
    always_on = (
      var.service_plan_sku_name != "F1" &&
      var.service_plan_sku_name != "Free"
    )

    application_stack {
      docker_registry_url      = var.docker_url
      docker_registry_username = var.docker_username
      docker_registry_password = var.docker_password
      docker_image_name        = var.image_name
    }

    api_definition_url = "https://${var.app_name}-${var.environment}-app.azurewebsites.net/swagger/index.html"
    ftps_state         = "FtpsOnly"
  }

  logs {
    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 35
      }
    }
  }

  app_settings = {
    ASPNETCORE_ENVIRONMENT                     = var.environment
    APPLICATIONINSIGHTS_CONNECTION_STRING      = var.app_insights_connection_string
    ApplicationInsightsAgent_EXTENSION_VERSION = "~3"
    XDT_MicrosoftApplicationInsights_Mode      = "Recommended"
    App__SelfUrl                               = "https://${var.app_name}-${var.environment}-app.azurewebsites.net"
    ConnectionStrings__ApplicationInsights     = var.app_insights_connection_string
    Application__Chat                          = var.chat_id
    Telegram__Chat                             = var.chat_id
    Telegram__Token                            = var.bot_token
    Telegram__SecretToken                      = var.secret_token
    Authorization__JwtBearer__Key              = var.authorization_key
    Authorization__JwtBearer__Audience         = var.app_name
    Authorization__JwtBearer__Issuer           = var.app_name
    Google__Credentials                        = var.google_credentials
    Sheets__SheetsId                           = var.sheets_id
    Sheets__SuggestionsRange                   = var.sheets_range
    Settings__IsRedisEnabled                   = var.settings_use_redis
    Azure__CosmosOptions__Endpoint             = var.cosmos_endpoint
    Azure__CosmosOptions__DatabaseId           = var.cosmos_database_id
    Azure__CosmosOptions__Container            = var.cosmos_database_container
    AzureAd__ClientId                          = var.azuread_client_id
    AzureAd__TenantId                          = var.tenant_id
    AzureAd__Urls__Authorization               = "https://login.microsoftonline.com/${var.tenant_id}/oauth2/v2.0/authorize"
    AzureAd__Urls__Token                       = "https://login.microsoftonline.com/${var.tenant_id}/oauth2/v2.0/token"
    AzureAd__Urls__Refresh                     = "https://login.microsoftonline.com/${var.tenant_id}/oauth2/v2.0/token"
  }

  sticky_settings {
    app_setting_names = [
      "ASPNETCORE_ENVIRONMENT",
      "APPLICATIONINSIGHTS_CONNECTION_STRING",
      "ApplicationInsightsAgent_EXTENSION_VERSION",
      "XDT_MicrosoftApplicationInsights_Mode"
    ]
  }

  identity {
    type = "SystemAssigned"
  }

  depends_on = [
    azurerm_service_plan.service_plan_api
  ]

  tags = {
    environment                              = var.environment
    management                               = "terraform"
    "hidden-link: /app-insights-conn-string" = var.app_insights_connection_string
  }
}
