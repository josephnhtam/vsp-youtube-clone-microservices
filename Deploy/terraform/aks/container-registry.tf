resource "azurerm_container_registry" "acr" {
  name                = var.acr.name
  resource_group_name = local.rg
  location            = var.location
  sku                 = var.acr.sku
  admin_enabled       = var.acr.admin_enabled
}

resource "kubernetes_secret" "image_pull_secret" {
  for_each = var.acr.admin_enabled && var.acr.create_pull_secret ? toset(["enabled"]) : toset([])

  metadata {
    name = var.acr.pull_secret_name
  }

  type = "kubernetes.io/dockerconfigjson"

  data = {
    ".dockerconfigjson" = jsonencode({
      auths = {
        "${azurerm_container_registry.acr.login_server}" = {
          "username" = azurerm_container_registry.acr.admin_username
          "password" = azurerm_container_registry.acr.admin_password
          "auth"     = base64encode("${azurerm_container_registry.acr.admin_username}:${azurerm_container_registry.acr.admin_password}")
        }
      }
    })
  }
}
