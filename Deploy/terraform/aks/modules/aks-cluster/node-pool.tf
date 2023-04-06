resource "azurerm_kubernetes_cluster_node_pool" "node_pools" {
  for_each = local.node_pools_map

  kubernetes_cluster_id = azurerm_kubernetes_cluster.kubernetes_cluster.id
  name                  = each.key
  mode                  = each.value.mode
  zones                 = each.value.zones
  enable_auto_scaling   = each.value.enable_auto_scaling
  node_count            = each.value.node_count
  max_count             = each.value.max_count
  min_count             = each.value.min_count
  os_disk_size_gb       = each.value.os_disk_size_gb
  vm_size               = each.value.vm_size
  os_type               = each.value.os_type
  node_labels           = each.value.node_labels
  node_taints           = each.value.node_taints
  tags                  = each.value.tags
  priority              = each.value.priority
  eviction_policy       = each.value.eviction_policy
  spot_max_price        = each.value.spot_max_price

  vnet_subnet_id = azurerm_subnet.node_pool_subnets[each.key].id

  depends_on = [
    azurerm_subnet.node_pool_subnets
  ]
}
