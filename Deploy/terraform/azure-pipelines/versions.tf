terraform {
  required_version = "~> 1.4.2"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.50.0"
    }

    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.37.0"
    }

    azuredevops = {
      source  = "microsoft/azuredevops"
      version = "~> 0.4.0"
    }
  }

  backend "azurerm" {}
}
