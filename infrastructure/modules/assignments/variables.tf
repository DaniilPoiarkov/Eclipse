variable "scope" {
  type        = string
  description = "value"
}

variable "role_definition_id" {
  type        = string
  description = "Role definition Id to be assigned"
}

variable "account_name" {
  type        = string
  description = "Cosmos db account name"
}

variable "principal_id" {
  type        = string
  description = "Client principal id"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group name"
}
