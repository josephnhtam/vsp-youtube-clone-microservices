resource "azuredevops_environment" "environments" {
  for_each = var.environments

  project_id  = azuredevops_project.project.id
  name        = each.key
  description = each.value.description
}
