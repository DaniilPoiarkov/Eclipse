provider "azurerm" {
  features {}

  subscription_id = "fdf82ff0-5e56-4964-b32d-f19fccad2b4d"
  environment     = "public"
  use_cli         = true
  use_oidc        = false
  use_msi         = false
}
