variable "name" {
  type        = string
  default     = "vsp"
  description = "Deployment name"
}

variable "location" {
  type        = string
  default     = "East Asia"
  description = "Location"
}

variable "vnet_address_space" {
  type    = string
  default = "10.0.0.0/8"
}

variable "kubernetes_version_prefix" {
  type        = string
  default     = "1.23"
  description = "A prefix filter for the versions of Kubernetes"
}

variable "aks_log_analytics_workspace" {
  type = object({
    sku               = optional(string, "PerGB2018")
    retention_in_days = optional(number, 30)
  })
  default = {}
}

variable "aks_default_node_pool" {
  type = object({
    name                = optional(string, "defaultnodepool")
    vm_size             = string
    enable_auto_scaling = optional(bool, false)
    node_count          = optional(number, 1)
    min_count           = optional(number, null)
    max_count           = optional(number, null)
    os_disk_size_gb     = optional(number, 30)
    zones               = optional(list(string), null)
    node_labels         = optional(map(string), {})

    subnet_name           = string
    subnet_address_prefix = string
  })

  default = {
    name                  = "defaultpool"
    enable_auto_scaling   = false
    node_count            = 1
    os_disk_size_gb       = 30
    vm_size               = "standard_e2pds_v5"
    subnet_name           = "default-nodes-subnet"
    subnet_address_prefix = "10.240.0.0/16"
  }
}

variable "aks_node_pools" {
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

  default = []
}

variable "aks_virtual_node_pool" {
  type = object({
    subnet_name           = string
    subnet_address_prefix = string
  })

  default = {
    subnet_name           = "virtual-nodes-subnet"
    subnet_address_prefix = "10.241.0.0/16"
  }
}

variable "aks_ssh_public_key_path" {
  type        = string
  default     = "./ssh-key/ssh-key.pub"
  description = "Path of the SSH public key"
}

variable "aks_ad_rbac_control" {
  type = object({
    enabled                = bool
    admin_group_object_ids = optional(list(string), [])
    azure_rbac_enabled     = optional(bool, false)
  })
  default = {
    enabled = false
  }
}
