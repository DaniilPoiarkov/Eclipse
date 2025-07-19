variable "subscription_id" {
  description = "subscription id"
  type        = string
  sensitive   = true
}

variable "location" {
  description = "Azure location for all resources"
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
  default     = "SHOULD_BE_SET"
}

variable "chat_id" {
  description = "Administrator chat id"
  type        = number
  sensitive   = true
}

variable "authorization_key" {
  description = "Secret key"
  type        = string
  sensitive   = true
  default     = "SHOULD_BE_SET"
}

variable "secret_token" {
  description = "Secret token"
  type        = string
  sensitive   = true
  default     = "SHOULD_BE_SET"
}

variable "sheets_id" {
  description = "Sheets id"
  type        = string
  sensitive   = true
  default     = "SHOULD_BE_SET"
}

variable "sheets_range" {
  description = "Suggestions sheets range"
  type        = string
  sensitive   = true
  default     = "SHOULD_BE_SET"
}

variable "settings_use_redis" {
  description = "Create and use redis cache"
  type        = bool
}

variable "google_credentials_path" {
  description = "Google credentials path"
  type        = string
  sensitive   = true
  default     = "SHOULD_BE_SET"
}

variable "docker_url" {
  description = "Docker url"
  type        = string
  default     = "SHOULD_BE_SET"
}

variable "docker_username" {
  description = "Docker username"
  type        = string
  sensitive   = true
  default     = "SHOULD_BE_SET"
}

variable "docker_password" {
  description = "Docker password"
  type        = string
  sensitive   = true
  default     = "SHOULD_BE_SET"
}

variable "cosmos_offer_type" {
  description = "Cosmos db offer type"
  type        = string
  default     = "Standard"
}

variable "cosmos_kind" {
  description = "Cosmos db kind. Allowed: GlobalDocumentDB, MongoDB, Parse. Default: GlobalDocumentDB"
  type        = string
  default     = "GlobalDocumentDB"
}

variable "cosmos_consistency" {
  description = "Cosmos db consistency"
  type        = string
  default     = "Session"
}

variable "cosmos_free_tier" {
  description = "Cosmos db free tier"
  type        = bool
  default     = true
}

variable "failover_priority" {
  description = "Failover priority for cosmosdb account"
  type        = number
}

variable "cosmos_container" {
  description = "Cosmos container"
  type        = string
}

variable "partiton_paths" {
  description = "Cosmos container partition paths"
  type        = list(string)
}

variable "partition_key_version" {
  description = "Partition key version"
  type        = number
}

variable "database_throughput" {
  description = "Database throughput"
  type        = number
}

variable "container_throughput" {
  description = "Container throughput"
  type        = number
}

variable "image_name" {
  description = "Image name"
  type        = string
}

variable "email_receiver" {
  description = "Email receiver"
  type        = string
}

variable "alert_severity" {
  type        = number
  description = "Severity level for custom alerts"
  default     = 1
}

variable "tg_alerts_chat" {
  type        = number
  description = "Telegram chat where alerts shold be sent"
  sensitive   = true
}

variable "tg_alert_bot_token" {
  type        = string
  description = "Telegram bot which should sent alerts"
  sensitive   = true
}
