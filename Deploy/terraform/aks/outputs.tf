output "virtual_network" {
  value = {
    name = azurerm_virtual_network.vnet.name
  }
}

output "aks_default_node_pool_subnet" {
  value = {
    name = module.aks_cluster.default_node_pool_subnet.name
  }
}

# output "aks_virtual_node_pool_subnet" {
#   value = {
#     name = module.aks_cluster.virtual_node_pool_subnet.name
#   }
# }

output "aks_node_pool_subnets" {
  value = [for s in module.aks_cluster.node_pool_subnets : {
    name = s.name
  }]
}

output "aks_log_analytics_workspace" {
  value = {
    name = module.aks_cluster.log_analytics_workspace.name
  }
}

output "aks_kubernetes_cluster" {
  value = {
    name    = module.aks_cluster.kubernetes_cluster.name
    version = module.aks_cluster.kubernetes_cluster.kubernetes_version
  }
}

output "acr" {
  value = {
    name         = length(azurerm_container_registry.acr) > 0 ? azurerm_container_registry.acr.0.name : ""
    login_server = length(azurerm_container_registry.acr) > 0 ? azurerm_container_registry.acr.0.login_server : ""
  }
}

output "static_public_ip" {
  value = [for i in azurerm_public_ip.static_public_ip : {
    name       = i.name
    ip_address = i.ip_address
  }]
}
