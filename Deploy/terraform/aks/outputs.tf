output "aks_default_node_pool_subnet" {
  value = module.aks_cluster.default_node_pool_subnet
}

output "aks_virtual_node_pool_subnet" {
  value = module.aks_cluster.virtual_node_pool_subnet
}

output "aks_node_pool_subnets" {
  value = module.aks_cluster.node_pool_subnets
}

output "aks_log_analytics_workspace" {
  value = module.aks_cluster.log_analytics_workspace
}

output "aks_cluster" {
  value = module.aks_cluster.aks_cluster
}

output "virtual_network" {
  value = {
    id   = azurerm_virtual_network.vnet.id
    name = azurerm_virtual_network.vnet.name
  }
}
