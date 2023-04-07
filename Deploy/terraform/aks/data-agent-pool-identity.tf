data "azurerm_user_assigned_identity" "agent_pool_identity" {
  resource_group_name = module.aks_cluster.node_resource_group_name
  name                = "${module.aks_cluster.kubernetes_cluster.name}-agentpool"

  depends_on = [
    module.aks_cluster
  ]
}
