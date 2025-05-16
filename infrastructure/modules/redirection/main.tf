resource "azuread_application_redirect_uris" "spa" {
  application_id = var.app_registration_id
  type           = "SPA"
  redirect_uris  = var.spa_redirect_uris
}
