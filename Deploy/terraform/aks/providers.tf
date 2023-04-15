provider "azurerm" {
  features {}
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
