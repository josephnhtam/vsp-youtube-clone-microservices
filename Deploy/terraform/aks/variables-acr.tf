variable "acr" {
  type = object({
    create              = bool
    name                = string
    resource_group_name = string
    sku                 = optional(string, "Basic")
    admin_enabled       = optional(bool, true)
    create_pull_secret  = optional(bool, false)
    pull_secret_name    = optional(string, "docker-config")
  })
  default = {
    create              = true
    name                = "vspsample"
    resource_group_name = "vspsample-acr-rg"
    admin_enabled       = true
    create_pull_secret  = false
  }
  description = "Configuration for container registry"
}
