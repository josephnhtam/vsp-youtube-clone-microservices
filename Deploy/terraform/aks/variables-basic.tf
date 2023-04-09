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

variable "dns_zone_name" {
  type        = string
  description = "DNS zone name"
  default     = "vspsample.online"
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
