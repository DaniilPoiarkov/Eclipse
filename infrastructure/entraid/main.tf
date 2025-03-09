data "azuread_client_config" "current" {}

resource "azuread_application_registration" "app" {
  display_name     = "${var.app_name} ${var.environment}"
  sign_in_audience = "AzureADMyOrg"
}

resource "azuread_application_owner" "owner" {
  application_id  = azuread_application_registration.app.id
  owner_object_id = data.azuread_client_config.current.object_id
}

resource "azuread_application_redirect_uris" "spa" {
  application_id = azuread_application_registration.app.id
  type           = "SPA"
  redirect_uris  = var.spa_redirect_uris
}

resource "random_uuid" "role_admin" {}

resource "azuread_application_app_role" "admin" {
  display_name   = "Admin"
  description    = "Super admin priveleged role"
  application_id = azuread_application_registration.app.id
  role_id        = random_uuid.role_admin.id

  allowed_member_types = [
    "User"
  ]
}

resource "random_uuid" "scope_default" {}

resource "azuread_application_permission_scope" "default" {
  application_id             = azuread_application_registration.app.id
  scope_id                   = random_uuid.scope_default.id
  value                      = "Default"
  admin_consent_description  = "Default"
  admin_consent_display_name = "Default"
}

data "azuread_application_published_app_ids" "well_known" {}

# resource "azuread_service_principal" "sp" {
#   client_id = data.azuread_application_published_app_ids.well_known.result["MicrosoftGraph"]
# }

# resource "azuread_application_api_access" "api_access" {
#   application_id = azuread_application_registration.app.id
#   api_client_id = data.azuread_application_published_app_ids.well_known.result["MicrosoftGraph"]

#   role_ids = [
#     data.azuread_service_principal.msgraph.app_role_ids["Group.Read.All"],
#     data.azuread_service_principal.msgraph.app_role_ids["User.Read.All"],
#   ]

#   scope_ids = [
#     data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["User.Read"]
#   ]
# }
