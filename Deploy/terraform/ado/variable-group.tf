resource "azuredevops_variable_group" "variable_group" {
  count = length(var.application_deployment_variables) > 0 ? 1 : 0

  project_id   = azuredevops_project.project.id
  name         = "application-variable-group"
  description  = "Variable group for application deployment"
  allow_access = true

  dynamic "variable" {
    for_each = [
      for key, value in var.application_deployment_variables : {
        key   = key
        value = value
      }
    ]

    content {
      is_secret = false
      name      = variable.value.key
      value     = variable.value.value
    }
  }
}
