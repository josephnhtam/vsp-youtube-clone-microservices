variable "name" {
  type        = string
  default     = "vspsample"
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

variable "dns_zone" {
  type = object({
    create              = bool
    name                = string
    resource_group_name = string
  })
  description = "DNS zone"
  default = {
    create              = true
    name                = "vspsample.online"
    resource_group_name = "vspsample-dns-zone-rg"
  }
}

variable "static_public_ip" {
  type = object({
    count    = optional(number, 0)
    sku_tier = optional(string, "Regional")
    zones    = optional(list(string), [])
  })
  default = {
    count = 0
  }
}
