resource "azurerm_subnet" "default_node_pool_subnet" {
  resource_group_name  = var.rg
  virtual_network_name = var.virtual_network
  name                 = var.default_node_pool.subnet_name
  address_prefixes     = [var.default_node_pool.subnet_address_prefix]
}

resource "azurerm_subnet" "node_pool_subnets" {
  for_each = local.node_pools_map

  resource_group_name  = var.rg
  virtual_network_name = var.virtual_network
  name                 = each.value.subnet_name
  address_prefixes     = [each.value.subnet_address_prefix]
}

# resource "azurerm_subnet" "virtual_node_pool_subnet" {
#   resource_group_name  = var.rg
#   virtual_network_name = var.virtual_network
#   name                 = var.virtual_node_pool.subnet_name
#   address_prefixes     = [var.virtual_node_pool.subnet_address_prefix]

#   delegation {
#     name = "aci-delegation"

#     service_delegation {
#       name    = "Microsoft.ContainerInstance/containerGroups"
#       actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
#     }
#   }
# }
