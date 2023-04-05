locals {
  node_resource_group          = "${var.name}-node-rg"
  log_analytics_workspace_Name = "${var.name}-insights"
  node_pools_map               = { for p in var.node_pools : p.name => p }
}
