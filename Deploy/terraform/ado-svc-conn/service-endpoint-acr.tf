data "azurerm_container_registry" "acr" {
  for_each = var.acr_create_service_connection ? toset(["enabled"]) : toset([])

  name                = var.acr_name
  resource_group_name = var.acr_resource_group
}

# resource "azuredevops_serviceendpoint_azurecr" "svc_conn" {
#   for_each = var.acr_create_service_connection ? toset(["enabled"]) : toset([])

#   project_id            = data.azuredevops_project.current.id
#   service_endpoint_name = var.acr_service_connection_name
#   resource_group        = var.acr_resource_group
#   azurecr_name          = var.acr_name

#   azurecr_spn_tenantid      = data.azurerm_client_config.current.tenant_id
#   azurecr_subscription_id   = data.azurerm_client_config.current.subscription_id
#   azurecr_subscription_name = data.azurerm_subscription.current.display_name
# }

resource "azuredevops_serviceendpoint_dockerregistry" "svc_conn" {
  for_each = data.azurerm_container_registry.acr

  project_id            = data.azuredevops_project.current.id
  service_endpoint_name = var.acr_service_connection_name
  docker_registry       = each.value.login_server
  registry_type         = "Others"

  docker_username = each.value.admin_username
  docker_password = each.value.admin_password
}

resource "azuredevops_resource_authorization" "azurecr" {
  for_each = azuredevops_serviceendpoint_dockerregistry.svc_conn

  project_id  = data.azuredevops_project.current.id
  resource_id = each.value.id
  authorized  = true
}
