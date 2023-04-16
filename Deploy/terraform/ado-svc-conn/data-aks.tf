data "azurerm_kubernetes_cluster" "cluster" {
  for_each = var.aks_create_service_connection ? toset(["enabled"]) : toset([])

  name                = var.aks_cluster_name
  resource_group_name = var.aks_resource_group
}
