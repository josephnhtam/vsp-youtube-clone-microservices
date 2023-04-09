resource "azurerm_dns_zone" "dns_zone" {
  count = var.dns_zone.create ? 1 : 0

  name                = var.dns_zone.name
  resource_group_name = var.dns_zone.resource_group_name

  depends_on = [
    azurerm_resource_group.dns_zone_rg
  ]
}

data "azurerm_dns_zone" "dns_zone" {
  name                = var.dns_zone.name
  resource_group_name = var.dns_zone.resource_group_name

  depends_on = [
    azurerm_dns_zone.dns_zone
  ]
}

resource "azurerm_role_assignment" "dns_zone_role_assignment" {
  scope                = data.azurerm_dns_zone.dns_zone.id
  principal_id         = data.azurerm_user_assigned_identity.agent_pool_identity.principal_id
  role_definition_name = "DNS Zone Contributor"
}
