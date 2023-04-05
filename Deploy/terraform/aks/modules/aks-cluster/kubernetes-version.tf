data "azurerm_kubernetes_service_versions" "version" {
  location        = var.location
  include_preview = false
  version_prefix  = var.kubernetes_version_prefix
}
