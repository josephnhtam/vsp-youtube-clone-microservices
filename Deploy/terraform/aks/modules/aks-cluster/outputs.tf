output "default_node_pool_subnet" {
  value = {
    id   = azurerm_subnet.default_node_pool_subnet.id
    name = azurerm_subnet.default_node_pool_subnet.name
  }
}

output "virtual_node_pool_subnet" {
  value = {
    id   = azurerm_subnet.virtual_node_pool_subnet.id
    name = azurerm_subnet.virtual_node_pool_subnet.name
  }
}

output "node_pool_subnets" {
  value = [for s in azurerm_subnet.node_pool_subnets : {
    id   = s.id
    name = s.name
  }]
}

output "log_analytics_workspace" {
  value = {
    id   = azurerm_log_analytics_workspace.workspace.id
    name = azurerm_log_analytics_workspace.workspace.name
  }
}

output "aks_cluster" {
  value = {
    id      = azurerm_kubernetes_cluster.aks_cluster.id,
    name    = azurerm_kubernetes_cluster.aks_cluster.name
    version = data.azurerm_kubernetes_service_versions.version.latest_version
  }
}
