data "azuredevops_users" "vsts" {
  origin = "vsts"
}

data "azuredevops_group" "endpoint_administrators" {
  project_id = azuredevops_project.project.id
  name       = "Endpoint Administrators"
}

resource "azuredevops_group_membership" "endpoint_administrators_membership" {
  group = data.azuredevops_group.endpoint_administrators.descriptor
  members = [
    for u in data.azuredevops_users.vsts.users : u.descriptor
    if startswith(u.display_name, "${azuredevops_project.project.name} Build Service")
  ]
}
