module "aks_cluster" {
  source = "./modules/aks-cluster"

  name            = local.aks_name
  rg              = azurerm_resource_group.rg.name
  location        = azurerm_resource_group.rg.location
  virtual_network = azurerm_virtual_network.vnet.name

  kubernetes_version_prefix = var.kubernetes_version_prefix
  default_node_pool         = var.aks_default_node_pool
  node_pools                = var.aks_node_pools
  # virtual_node_pool         = var.aks_virtual_node_pool
  ssh_public_key_path     = var.aks_ssh_public_key_path
  ad_rbac_control         = var.aks_ad_rbac_control
  log_analytics_workspace = var.aks_log_analytics_workspace
  sku_tier                = var.aks_sku_tier
  load_balancer_sku       = var.aks_load_balancer_sku

  web_app_routing = {
    enabled     = var.aks_use_web_app_routing
    dns_zone_id = azurerm_dns_zone.dns_zone.id
  }
}

provider "kubernetes" {
  host                   = module.aks_cluster.kubernetes_cluster.kube_config.0.host
  username               = module.aks_cluster.kubernetes_cluster.kube_config.0.username
  password               = module.aks_cluster.kubernetes_cluster.kube_config.0.password
  client_certificate     = base64decode(module.aks_cluster.kubernetes_cluster.kube_config.0.client_certificate)
  client_key             = base64decode(module.aks_cluster.kubernetes_cluster.kube_config.0.client_key)
  cluster_ca_certificate = base64decode(module.aks_cluster.kubernetes_cluster.kube_config.0.cluster_ca_certificate)
}

provider "helm" {
  kubernetes {
    host                   = module.aks_cluster.kubernetes_cluster.kube_config.0.host
    username               = module.aks_cluster.kubernetes_cluster.kube_config.0.username
    password               = module.aks_cluster.kubernetes_cluster.kube_config.0.password
    client_certificate     = base64decode(module.aks_cluster.kubernetes_cluster.kube_config.0.client_certificate)
    client_key             = base64decode(module.aks_cluster.kubernetes_cluster.kube_config.0.client_key)
    cluster_ca_certificate = base64decode(module.aks_cluster.kubernetes_cluster.kube_config.0.cluster_ca_certificate)
  }
}
