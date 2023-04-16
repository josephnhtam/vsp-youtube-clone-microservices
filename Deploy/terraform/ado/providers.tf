provider "azurerm" {
  features {}
}

provider "azuredevops" {
  org_service_url       = var.ado_org_service_url
  personal_access_token = var.ado_personal_access_token
}
