resource "azurerm_dns_zone" "dns_zone" {
  name                = var.dns_zone_name
  resource_group_name = azurerm_resource_group.rg.name
}

resource "azurerm_role_assignment" "dns_zone_role_assignment" {
  scope                = azurerm_dns_zone.dns_zone.id
  principal_id         = data.azurerm_user_assigned_identity.agent_pool_identity.principal_id
  role_definition_name = "DNS Zone Contributor"

  depends_on = [
    azurerm_dns_zone.dns_zone
  ]
}
