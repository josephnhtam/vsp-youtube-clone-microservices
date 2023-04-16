variable "org_service_url" {
  type        = string
  description = "The Azure DevOps organization url"
  sensitive   = true
}

variable "personal_access_token" {
  type        = string
  description = "The Azure DevOps organization personal access token"
  sensitive   = true
}

variable "project_name" {
  type        = string
  description = "The name of the Azure DevOps project"
}
