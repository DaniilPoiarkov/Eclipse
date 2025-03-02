variable "resource_group_name" {
  type = string
  description = "Resource group"
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

variable "cosmos_offer_type" {
  description = "Cosmos db offer type"
  type        = string
}

variable "cosmos_kind" {
  description = "Cosmos db kind. Allowed: GlobalDocumentDB, MongoDB, Parse. Default: GlobalDocumentDB"
  type        = string
}

variable "cosmos_consistency" {
  description = "Cosmos db consistency"
  type        = string
}

variable "cosmos_free_tier" {
  description = "Cosmos db free tier"
  type        = bool
}

variable "failover_priority" {
  description = "Failover priority for cosmosdb account"
  type        = number
}

variable "container_name" {
  description = "Container name"
  type        = string
}

variable "partition_paths" {
  description = "Container partitions"
  type        = list(string)
}

variable "database_throughput" {
  description = "Database throughput"
  type        = number
}

variable "container_throughput" {
  description = "Container throughput"
  type        = number
}
