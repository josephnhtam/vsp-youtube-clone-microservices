variable "ado_org_name" {
  type        = string
  description = "The Azure DevOps organization name"
  sensitive   = true
}

variable "ado_org_service_url" {
  type        = string
  description = "The Azure DevOps organization url"
  sensitive   = true
}

variable "ado_personal_access_token" {
  type        = string
  description = "The Azure DevOps organization personal access token"
  sensitive   = true
}

variable "ado_project_name" {
  type        = string
  description = "Project name"
  default     = "vspsample-devops"
}

variable "ado_project_description" {
  type        = string
  description = "Project description"
  default     = "vspsample-devops"
}

variable "ado_project_visibility" {
  type        = string
  description = "Project visibility. private or public."
  default     = "private"
}

variable "ado_work_item_template" {
  type        = string
  description = "Work item template"
  default     = "Agile"
}
