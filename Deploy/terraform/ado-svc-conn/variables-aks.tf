variable "aks_create_service_connection" {
  type        = bool
  description = "Whether use service connection for aks"
  default     = true
}

variable "aks_service_connection_name" {
  type        = string
  description = "The name of the aks service connection"
  default     = "dev-kubernetes-service-connection"
}

variable "aks_namespace" {
  type        = string
  description = "The namespace of the aks cluster"
  default     = "default"
}

variable "aks_cluster_admin" {
  type        = bool
  description = "Whether use cluster admin credentials"
  default     = true
}

variable "aks_cluster_name" {
  type        = string
  description = "The name of the aks cluster"
  default     = "vspsample-aks"
}

variable "aks_resource_group" {
  type        = string
  description = "The resource group name of the aks cluster"
  default     = "vspsample-rg"
}
