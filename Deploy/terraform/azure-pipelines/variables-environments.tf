variable "environments" {
  type = map(object(
    {
      description = optional(string, "")
    }
  ))
  default = {
    "dev-environment" = {
      description = "Environment for dev application deployment"
    }

    "dev-infrastructure-deployment" = {
      description = "Environment for dev infrastructure deployment"
    }
  }
}
