variable "resource_group_name" {
  type        = string
  description = "Resource group"
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

variable "application_type" {
  description = "App Insights application type"
  type        = string
}
