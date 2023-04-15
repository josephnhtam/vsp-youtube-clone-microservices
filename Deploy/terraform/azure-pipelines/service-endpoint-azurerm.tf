resource "azuread_application" "azure_service_connection" {
  display_name = local.azure_service_connection_name
}

resource "azuread_service_principal" "azure_service_connection" {
  application_id = azuread_application.azure_service_connection.application_id
}

resource "azuread_application_password" "azure_service_connection" {
  application_object_id = azuread_application.azure_service_connection.object_id
  end_date_relative     = var.azure_client_secret_end_date_relative
}

resource "azurerm_role_assignment" "azure_service_connection" {
  scope                = data.azurerm_subscription.current.id
  principal_id         = azuread_service_principal.azure_service_connection.object_id
  role_definition_name = var.azure_service_principal_role
}

resource "azuredevops_serviceendpoint_azurerm" "azurerm" {
  project_id            = azuredevops_project.project.id
  service_endpoint_name = local.azure_service_connection_name
  description           = "Azure service connection for provisioning infrastructure"

  azurerm_spn_tenantid      = data.azurerm_client_config.current.tenant_id
  azurerm_subscription_id   = data.azurerm_client_config.current.subscription_id
  azurerm_subscription_name = data.azurerm_subscription.current.display_name

  credentials {
    serviceprincipalid  = azuread_service_principal.azure_service_connection.application_id
    serviceprincipalkey = azuread_application_password.azure_service_connection.value
  }
}

resource "azuredevops_resource_authorization" "azurerm" {
  project_id  = azuredevops_project.project.id
  resource_id = azuredevops_serviceendpoint_azurerm.azurerm.id
  authorized  = true
}
