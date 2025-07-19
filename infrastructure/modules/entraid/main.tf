resource "azuread_application_registration" "app" {
  display_name     = var.app_name
  sign_in_audience = "AzureADMyOrg"
}

resource "azuread_application_owner" "owner" {
  application_id  = azuread_application_registration.app.id
  owner_object_id = var.owner_object_id

  lifecycle {
    ignore_changes = [
      application_id
    ]
  }
}

resource "random_uuid" "role_admin" {}

resource "azuread_application_app_role" "admin" {
  display_name   = "Admin"
  value          = "Admin"
  description    = "Super admin priveleged role"
  application_id = azuread_application_registration.app.id
  role_id        = random_uuid.role_admin.id

  allowed_member_types = [
    "User"
  ]

  lifecycle {
    ignore_changes = [
      role_id,
      application_id
    ]
  }
}

resource "random_uuid" "scope_default" {}

resource "azuread_application_permission_scope" "default" {
  application_id             = azuread_application_registration.app.id
  scope_id                   = random_uuid.scope_default.id
  value                      = "Default"
  admin_consent_description  = "Default"
  admin_consent_display_name = "Default"
  
  lifecycle {
    ignore_changes = [
      scope_id,
      application_id
    ]
  }
}

data "azuread_application_published_app_ids" "well_known" {}

data "azuread_service_principal" "msgraph" {
  client_id = data.azuread_application_published_app_ids.well_known.result.MicrosoftGraph
}

resource "azuread_application_api_access" "api_access" {
  application_id = azuread_application_registration.app.id
  api_client_id  = data.azuread_application_published_app_ids.well_known.result.MicrosoftGraph

  scope_ids = [
    data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["User.Read"],
    data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["email"],
    data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["offline_access"],
    data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["openid"],
    data.azuread_service_principal.msgraph.oauth2_permission_scope_ids["profile"]
  ]

  lifecycle {
    ignore_changes = [
      application_id
    ]
  }
}
