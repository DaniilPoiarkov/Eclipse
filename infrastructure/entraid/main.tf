resource "azuread_application_registration" "app" {
  display_name = "${var.app_name} ${var.environment}"
}

resource "azuread_application_redirect_uris" "spa" {
  application_id = azuread_application_registration.app.id
  type           = "SPA"
  redirect_uris  = var.spa_redirect_uris
}

resource "random_uuid" "uuid" {}

resource "azuread_application_app_role" "admin" {
  display_name   = "Admin"
  description    = "Suped admin priveleged role"
  application_id = azuread_application_registration.app.id
  role_id        = random_uuid.uuid.id

  allowed_member_types = [
    "User"
  ]
}
