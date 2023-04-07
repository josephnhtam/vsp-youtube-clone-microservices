variable "acr" {
  type = object({
    enabled            = bool
    name               = string
    sku                = optional(string, "Basic")
    admin_enabled      = optional(bool, true)
    create_pull_secret = optional(bool, false)
    pull_secret_name   = optional(string, "docker-config")
  })
  default = {
    enabled            = true
    name               = "vspsample"
    admin_enabled      = true
    create_pull_secret = false
  }
  description = "Configuration for container registry"
}
