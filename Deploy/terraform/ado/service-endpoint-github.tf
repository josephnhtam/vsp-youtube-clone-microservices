resource "azuredevops_serviceendpoint_github" "github" {
  project_id            = azuredevops_project.project.id
  service_endpoint_name = "github-service-connection"
  description           = "The GitHub service connection for repository"

  auth_personal {
    personal_access_token = var.github_personal_access_token
  }
}

resource "azuredevops_resource_authorization" "github" {
  project_id  = azuredevops_project.project.id
  resource_id = azuredevops_serviceendpoint_github.github.id
  authorized  = true
}
