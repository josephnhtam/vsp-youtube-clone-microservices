variable "acr" {
  type = object({
    create              = bool
    name                = string
    resource_group_name = string
    sku                 = optional(string, "Basic")
    create_pull_secret  = optional(bool, true)
    pull_secret_name    = optional(string, "docker-config")
  })
  default = {
    create              = true
    name                = "vspsample"
    resource_group_name = "vspsample-acr-rg"
  }
  description = "Configuration for container registry"
}

