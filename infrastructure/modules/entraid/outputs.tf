output "app_client_id" {
  value = azuread_application_registration.app.client_id
}

output "app_registration_id" {
  value = azuread_application_registration.app.id
}
