resource "helm_release" "external_dns" {
  count = var.external_dns.install ? 1 : 0

  name       = "external-dns"
  repository = "https://charts.bitnami.com/bitnami"
  chart      = "external-dns"

  create_namespace = true
  namespace        = var.external_dns.namespace

  version = var.external_dns.version

  set {
    name  = "provider"
    value = "azure"
  }

  set {
    name  = "txtOwnerId"
    value = module.aks_cluster.kubernetes_cluster.name
  }

  set {
    name  = "azure.resourceGroup"
    value = var.dns_zone.resource_group_name
  }

  set {
    name  = "azure.tenantId"
    value = data.azurerm_client_config.client_config.tenant_id
  }

  set {
    name  = "azure.subscriptionId"
    value = data.azurerm_client_config.client_config.subscription_id
  }

  set {
    name  = "azure.useManagedIdentityExtension"
    value = "true"
  }

  set {
    name  = "azure.userAssignedIdentityID"
    value = data.azurerm_user_assigned_identity.agent_pool_identity.client_id
  }

  dynamic "set" {
    for_each = var.external_dns.values

    content {
      name  = set.key
      value = set.value
    }
  }
}
