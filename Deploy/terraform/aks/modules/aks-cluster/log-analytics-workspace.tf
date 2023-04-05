resource "azurerm_log_analytics_workspace" "workspace" {
  name                = local.log_analytics_workspace_Name
  resource_group_name = var.rg
  location            = var.location
  sku                 = var.log_analytics_workspace.sku
  retention_in_days   = var.log_analytics_workspace.retention_in_days
}
