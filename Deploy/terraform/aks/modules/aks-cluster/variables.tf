variable "name" {
  type        = string
  description = "Aks cluster name"
}

variable "rg" {
  type        = string
  description = "Resource group name"
}

variable "location" {
  type        = string
  description = "Location"
}

variable "virtual_network" {
  type        = string
  description = "Virtual network name"
}

variable "kubernetes_version_prefix" {
  type        = string
  default     = null
  description = "A prefix filter for the versions of Kubernetes"
}

variable "sku_tier" {
  type        = string
  default     = "Free"
  description = "The SKU Tier that should be used for this Kubernetes Cluster"
}

variable "load_balancer_sku" {
  type        = string
  default     = "Standard"
  description = "Specifies the SKU of the Load Balancer used for this Kubernetes Cluster"
}

variable "ad_rbac_control" {
  type = object({
    enabled                = bool
    admin_group_object_ids = optional(list(string), [])
    azure_rbac_enabled     = optional(bool, false)
  })
  default = {
    enabled = false
  }
}

variable "log_analytics_workspace" {
  type = object({
    sku               = optional(string, "PerGB2018")
    retention_in_days = optional(number, 30)
  })
  default = {}
}

variable "ssh_public_key_path" {
  type        = string
  description = "Path of the SSH public key"
}

variable "user_assigned_manage_identity_ids" {
  type        = list(string)
  description = "Ids of the user assigned managed identity"
  default     = []
}

variable "default_node_pool" {
  type = object({
    name                = optional(string, "defaultpool")
    vm_size             = string
    enable_auto_scaling = optional(bool, false)
    node_count          = optional(number, 1)
    min_count           = optional(number, null)
    max_count           = optional(number, null)
    os_disk_size_gb     = optional(number, 30)
    zones               = optional(list(string), null)
    node_labels         = optional(map(string), {})
    node_taints         = optional(list(string), null)
    tags                = optional(map(string), {})

    subnet_name           = string
    subnet_address_prefix = string
  })
}

variable "node_pools" {
  type = list(object({
    name                = string
    vm_size             = string
    mode                = optional(string, "User")
    os_type             = optional(string, "Linux")
    enable_auto_scaling = optional(bool, false)
    node_count          = optional(number, 1)
    min_count           = optional(number, null)
    max_count           = optional(number, null)
    os_disk_size_gb     = optional(number, 30)
    zones               = optional(list(string), null)
    node_labels         = optional(map(string), {})
    node_taints         = optional(list(string), null)
    tags                = optional(map(string), {})
    priority            = optional(string, "Regular")
    eviction_policy     = optional(string, null)
    spot_max_price      = optional(number, null)

    subnet_name           = string
    subnet_address_prefix = string
  }))
}

# variable "virtual_node_pool" {
#   type = object({
#     subnet_name           = string
#     subnet_address_prefix = string
#   })
# }

variable "web_app_routing" {
  type = object({
    enabled     = bool
    dns_zone_id = optional(string, null)
  })
  default = {
    enabled = false
  }
}
