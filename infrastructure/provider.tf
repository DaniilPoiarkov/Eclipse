provider "azurerm" {
  features {}

  subscription_id = var.subscription_id
  environment     = "public"
  use_cli         = true
  use_oidc        = false
  use_msi         = false
}
