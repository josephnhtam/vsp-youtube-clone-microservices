terraform {
  required_version = "~> 1.4.2"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.50.0"
    }

    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.19.0"
    }

    helm = {
      source  = "hashicorp/helm"
      version = "~> 2.9.0"
    }
  }

  backend "azurerm" {}
}
