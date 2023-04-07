variable "prometheus" {
  type = object({
    install         = bool
    namespace       = optional(string, "monitoring")
    stack_version   = optional(string, null)
    stack_values    = map(string)
    adapter_version = optional(string, null)
    adapter_values  = map(string)
  })
  default = {
    install         = true
    stack_version   = "45.8.1"
    stack_values    = {}
    adapter_version = "4.1.1"
    adapter_values  = {}
  }
}

variable "ingress_nginx" {
  type = object({
    install   = bool
    namespace = optional(string, "ingress")
    version   = optional(string, null)
    values    = map(string)
  })
  default = {
    install = true
    version = "4.5.2"
    values = {
      "controller.service.externalTrafficPolicy" = "Local"
      "controller.autoscaling.enable"            = "true"
      "controller.autoscaling.minReplicas"       = "1"
      "controller.autoscaling.maxReplicas"       = "2"
    }
  }
}

variable "external_dns" {
  type = object({
    install   = bool
    namespace = optional(string, "ingress")
    version   = optional(string, null)
    values    = map(string)
  })
  default = {
    install = true
    version = "6.17.0"
    values = {
      "policy" = "sync"
    }
  }
}

variable "cert_manager" {
  type = object({
    install   = bool
    namespace = optional(string, "ingress")
    version   = optional(string, null)
    values    = map(string)
  })
  default = {
    install = true
    version = "v1.11.1"
    values  = {}
  }
}
