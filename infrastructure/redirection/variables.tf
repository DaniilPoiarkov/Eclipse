variable "app_registration_id" {
  description = "App registration id"
  type        = string
}

variable "spa_redirect_uris" {
  description = "Redirect uris for SPA"
  type        = list(string)
}
