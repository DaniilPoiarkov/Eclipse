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
