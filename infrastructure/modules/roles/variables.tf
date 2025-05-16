variable "resource_group_name" {
  description = "Resource group name"
  type        = string
}

variable "cosmos_principal" {
  description = "Identity to which assign cosmos access"
  type        = string
}

variable "scope" {
  description = "Assignment scope"
  type        = string
}

variable "assignable_scopes" {
  description = "Assignable scopes"
  type        = list(string)
}

variable "account_name" {
  description = "Cosmos db account name"
  type        = string
}
