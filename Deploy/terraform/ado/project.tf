resource "azuredevops_project" "project" {
  name               = var.ado_project_name
  description        = var.ado_project_description
  visibility         = var.ado_project_visibility
  work_item_template = var.ado_work_item_template
  version_control    = "Git"

  features = {
    "pipelines"    = "enabled",
    "artifacts"    = "enabled",
    "testplans"    = "enabled",
    "boards"       = "enabled",
    "repositories" = "disabled"
  }
}
