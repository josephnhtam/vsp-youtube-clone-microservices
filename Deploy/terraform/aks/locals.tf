locals {
  rg                            = "${var.name}-rg"
  log_analytics_workspace_name  = "${var.name}-insights"
  vnet_name                     = "${var.name}-vnet"
  aks_name                      = "${var.name}-aks"
  aks_subnet_name               = "${var.name}-aks-subnet"
  aks_virtual_nodes_subnet_name = "${var.name}-aks-virtual-nodes-subnet"
  identity_name                 = "${var.name}-identity"
}
