variable "acr_create_service_connection" {
  type        = bool
  description = "Whether create service connection of acr in Azure DevOps"
  default     = true
}

variable "acr_service_connection_name" {
  type        = string
  description = "The name of the acr service connection"
  default     = "dev-docker-registry-service-connection"
}

variable "acr_name" {
  type        = string
  description = "The name of the acr"
  default     = "vspsample"
}

variable "acr_resource_group" {
  type        = string
  description = "The resource group name of the acr"
  default     = "vspsample-acr-rg"
}
