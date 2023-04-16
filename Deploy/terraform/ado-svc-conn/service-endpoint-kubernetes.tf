resource "azuredevops_serviceendpoint_kubernetes" "svc_conn" {
  for_each = data.azurerm_kubernetes_cluster.cluster

  project_id            = data.azuredevops_project.current.id
  service_endpoint_name = var.aks_service_connection_name
  apiserver_url         = each.value.kube_config.0.host

  authorization_type = "Kubeconfig"
  kubeconfig {
    kube_config = each.value.kube_config_raw
  }

  # authorization_type    = "AzureSubscription"
  # azure_subscription {
  #   subscription_id   = data.azurerm_client_config.current.subscription_id
  #   subscription_name = data.azurerm_subscription.current.display_name
  #   tenant_id         = data.azurerm_client_config.current.tenant_id
  #   resourcegroup_id  = var.aks_resource_group
  #   namespace         = var.aks_namespace
  #   cluster_name      = var.aks_cluster_name
  # }
}

resource "azuredevops_resource_authorization" "kubernetes" {
  for_each = azuredevops_serviceendpoint_kubernetes.svc_conn

  project_id  = data.azuredevops_project.current.id
  resource_id = each.value.id
  authorized  = true
}
