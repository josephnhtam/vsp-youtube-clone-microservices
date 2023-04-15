resource "azuredevops_build_definition" "pipelines" {
  for_each = var.pipelines

  project_id = azuredevops_project.project.id
  name       = each.key


  ci_trigger {
    use_yaml = true
  }

  repository {
    repo_type             = "GitHub"
    repo_id               = var.github_repo
    branch_name           = var.github_repo_default_branch
    service_connection_id = azuredevops_serviceendpoint_github.github.id
    yml_path              = each.value["yml_path"]
  }

  depends_on = [
    azuredevops_resource_authorization.azurerm,
    azuredevops_resource_authorization.github,
    azuredevops_environment.environments
  ]
}
