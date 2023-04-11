resource "azurerm_resource_group" "rg" {
  name     = local.rg
  location = var.location
}

resource "azurerm_resource_group" "dns_zone_rg" {
  count = var.dns_zone.create && var.dns_zone.resource_group_name != local.rg ? 1 : 0

  name     = var.dns_zone.resource_group_name
  location = var.location
}

resource "azurerm_resource_group" "acr_rg" {
  count = var.acr.create && var.acr.resource_group_name != local.rg ? 1 : 0

  name     = var.acr.resource_group_name
  location = var.location
}
