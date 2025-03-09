variable "app_name" {
  description = "Application name"
  type        = string
}

variable "environment" {
  description = "Environment"
  type        = string
}

variable "spa_redirect_uris" {
  description = "SPA app redirect uris"
  type        = list(string)
}
