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

variable "dependency_target" {
  type        = string
  description = "Dependency target which will be used for log-based alert"
}
