output "default_node_pool_subnet" {
  value = azurerm_subnet.default_node_pool_subnet
}

# output "virtual_node_pool_subnet" {
#   value = azurerm_subnet.virtual_node_pool_subnet
# }

output "node_pool_subnets" {
  value = [for s in azurerm_subnet.node_pool_subnets : s]
}

output "log_analytics_workspace" {
  value = azurerm_log_analytics_workspace.workspace
}

output "kubernetes_cluster" {
  value = azurerm_kubernetes_cluster.kubernetes_cluster
}

output "node_resource_group_name" {
  value = local.node_resource_group
}
