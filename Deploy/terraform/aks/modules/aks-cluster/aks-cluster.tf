resource "azurerm_kubernetes_cluster" "kubernetes_cluster" {
  name                 = var.name
  resource_group_name  = var.rg
  location             = var.location
  dns_prefix           = var.name
  node_resource_group  = local.node_resource_group
  kubernetes_version   = data.azurerm_kubernetes_service_versions.version.latest_version
  azure_policy_enabled = true

  oms_agent {
    log_analytics_workspace_id = azurerm_log_analytics_workspace.workspace.id
  }

  dynamic "web_app_routing" {
    for_each = var.web_app_routing.enabled ? toset(["enabled"]) : toset([])
    content {
      dns_zone_id = var.web_app_routing.dns_zone_id
    }
  }

  dynamic "azure_active_directory_role_based_access_control" {
    for_each = var.ad_rbac_control.enabled ? toset(["enabled"]) : toset([])
    content {
      managed                = true
      admin_group_object_ids = var.ad_rbac_control.admin_group_object_ids
      azure_rbac_enabled     = var.ad_rbac_control.azure_rbac_enabled
    }
  }

  default_node_pool {
    name                = var.default_node_pool.name
    vnet_subnet_id      = azurerm_subnet.default_node_pool_subnet.id
    vm_size             = var.default_node_pool.vm_size
    enable_auto_scaling = var.default_node_pool.enable_auto_scaling
    node_count          = !var.default_node_pool.enable_auto_scaling ? var.default_node_pool.node_count : null
    min_count           = var.default_node_pool.enable_auto_scaling ? var.default_node_pool.min_count : null
    max_count           = var.default_node_pool.enable_auto_scaling ? var.default_node_pool.max_count : null
    os_disk_size_gb     = var.default_node_pool.os_disk_size_gb
    zones               = var.default_node_pool.zones
    node_labels         = var.default_node_pool.node_labels
    node_taints         = var.default_node_pool.node_taints
  }

  linux_profile {
    admin_username = "ubuntu"

    ssh_key {
      key_data = file(var.ssh_public_key_path)
    }
  }

  network_profile {
    network_plugin = "azure"
  }

  identity {
    type         = length(var.user_assigned_manage_identity_ids) == 0 ? "SystemAssigned" : "UserAssigned"
    identity_ids = length(var.user_assigned_manage_identity_ids) == 0 ? null : var.user_assigned_manage_identity_ids
  }

  # aci_connector_linux {
  #   subnet_name = azurerm_subnet.virtual_node_pool_subnet.name
  # }

  # provisioner "local-exec" {
  #   when    = create
  #   command = "az aks enable-addons -n ${var.name} -g ${var.rg} --addons virtual-node --subnet-name ${azurerm_subnet.virtual_node_pool_subnet.name}"
  # }

  provisioner "local-exec" {
    when    = create
    command = "az aks update --enable-blob-driver -n ${var.name} -g ${var.rg} --y"
  }
}
