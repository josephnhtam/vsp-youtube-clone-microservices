resource "azurerm_public_ip" "static_public_ip" {
  count = var.static_public_ip.count

  name                = "${var.name}-ip-${count.index}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = var.aks_load_balancer_sku
  sku_tier            = var.static_public_ip.sku_tier
  zones               = var.static_public_ip.zones
  allocation_method   = "Static"
}
