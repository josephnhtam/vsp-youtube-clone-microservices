resource "azurerm_container_registry" "acr" {
  count = var.acr.create ? 1 : 0

  name                = var.acr.name
  resource_group_name = var.acr.resource_group_name
  location            = var.location
  sku                 = var.acr.sku
  admin_enabled       = true

  depends_on = [
    azurerm_resource_group.acr_rg
  ]
}

data "azurerm_container_registry" "acr" {
  name                = var.acr.name
  resource_group_name = var.acr.resource_group_name

  depends_on = [
    azurerm_container_registry.acr
  ]
}

resource "kubernetes_secret" "image_pull_secret" {
  count = var.acr.create_pull_secret ? 1 : 0

  metadata {
    name = var.acr.pull_secret_name
  }

  type = "kubernetes.io/dockerconfigjson"

  data = {
    ".dockerconfigjson" = jsonencode({
      auths = {
        "${data.azurerm_container_registry.acr.login_server}" = {
          "username" = data.azurerm_container_registry.acr.admin_username
          "password" = data.azurerm_container_registry.acr.admin_password
          "auth"     = base64encode("${data.azurerm_container_registry.acr.admin_username}:${data.azurerm_container_registry.acr.admin_password}")
        }
      }
    })
  }
}

resource "azurerm_role_assignment" "acr_role_assignment" {
  scope                = data.azurerm_container_registry.acr.id
  principal_id         = data.azurerm_user_assigned_identity.agent_pool_identity.principal_id
  role_definition_name = "AcrPull"
}
