variable "environment" {
  description = "Environment"
  type        = string
}

variable "location" {
  description = "Azure location for all resources"
  type        = string
}

variable "email_receiver" {
  description = "Email receiver"
  type        = string
}

variable "scope_resource_ids" {
  type        = list(string)
  description = "Resources for which alert tules should be applied."
}

variable "data_source_id" {
  type        = string
  description = "Source for log-based alerts"
}

variable "severity" {
  type        = number
  description = "Severity level for log-based alerts"
}

variable "dependency_targets" {
  type        = set(string)
  description = "Dependency targets which will be used for log-based alert"
}

variable "tg_allert_bot_token" {
  type        = string
  description = "Bot token to sent alerts"
  sensitive   = true
}

variable "tg_alerts_chat" {
  type        = number
  description = "Chat where tg alerts should be sent"
  sensitive   = true
}
