variable "resource_group_name" {
  description = "Resource group"
  type        = string
}

variable "environment" {
  description = "Environment"
  type        = string
}

variable "app_name" {
  description = "Application name"
  type        = string
}

variable "location" {
  description = "Azure location for all resources"
  type        = string
}

variable "service_plan_sku_name" {
  description = "App service plan"
  type        = string
}

variable "os_type" {
  description = "App service plan os"
  type        = string
  default     = "Linux"
}

variable "bot_token" {
  description = "Bot token"
  type        = string
  sensitive   = true
}

variable "chat_id" {
  description = "Administrator chat id"
  type        = number
  sensitive   = true
}

variable "authorization_key" {
  description = "Secret key for jwt bearer authorization"
  type        = string
  sensitive   = true
}

variable "secret_token" {
  description = "Secret token used for authenticate telegram requests."
  type        = string
  sensitive   = true
}

variable "sheets_id" {
  description = "Sheets id"
  type        = string
  sensitive   = true
}

variable "sheets_range" {
  description = "Suggestions sheets range"
  type        = string
  sensitive   = true
}

variable "settings_use_redis" {
  description = "Create and use redis cache"
  type        = bool
}

variable "google_credentials" {
  description = "Google credentials"
  type        = string
  sensitive   = true
}

variable "docker_url" {
  description = "Docker url"
  type        = string
}

variable "docker_username" {
  description = "Docker username"
  type        = string
  sensitive   = true
}

variable "docker_password" {
  description = "Docker password"
  type        = string
  sensitive   = true
}

variable "app_insights_connection_string" {
  description = "AppInsights connection string"
  type        = string
  sensitive   = true
}

variable "identities" {
  description = "Identity ids"
  type        = list(string)
  default     = []
}

variable "cosmos_endpoint" {
  description = "CosmosDb endpoint"
  type        = string
}

variable "cosmos_database_id" {
  description = "Cosmos database id"
  type        = string
}

variable "cosmos_database_container" {
  description = "Cosmos container"
  type        = string
}
