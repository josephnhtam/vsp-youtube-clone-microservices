terraform {
  required_version = "~> 1.4.2"

  required_providers {
    azuredevops = {
      source  = "microsoft/azuredevops"
      version = "~> 0.4.0"
    }

    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.50.0"
    }
  }

  backend "azurerm" {}
}
