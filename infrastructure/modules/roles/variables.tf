variable "resource_group_name" {
  description = "Resource group name"
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
