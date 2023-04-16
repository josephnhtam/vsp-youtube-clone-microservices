variable "pipelines" {
  type = map(object({
    yml_path = string
  }))

  default = {
    "infrastructure-pipeline" = {
      yml_path = "AzurePipelines/infrastructure-pipeline.yml"
    }

    "common-pipeline" = {
      yml_path = "AzurePipelines/common-pipeline.yml"
    }

    "api-gateway-pipeline" = {
      yml_path = "AzurePipelines/api-gateway-pipeline.yml"
    }

    "community-api-pipeline" = {
      yml_path = "AzurePipelines/community-api-pipeline.yml"
    }

    "history-api-pipeline" = {
      yml_path = "AzurePipelines/history-api-pipeline.yml"
    }

    "identity-provider-pipeline" = {
      yml_path = "AzurePipelines/identity-provider-pipeline.yml"
    }

    "infrastructure-pipeline" = {
      yml_path = "AzurePipelines/infrastructure-pipeline.yml"
    }

    "library-api-pipeline" = {
      yml_path = "AzurePipelines/library-api-pipeline.yml"
    }

    "search-api-pipeline" = {
      yml_path = "AzurePipelines/search-api-pipeline.yml"
    }

    "storage-api-pipeline.yml" = {
      yml_path = "AzurePipelines/storage-api-pipeline.yml"
    }

    "subscriptions-api-pipeline" = {
      yml_path = "AzurePipelines/subscriptions-api-pipeline.yml"
    }

    "users-api-pipeline" = {
      yml_path = "AzurePipelines/users-api-pipeline.yml"
    }

    "video-manager-api" = {
      yml_path = "AzurePipelines/video-manager-api.yml"
    }

    "video-manager-signalrhub" = {
      yml_path = "AzurePipelines/video-manager-signalrhub.yml"
    }

    "video-processor-application-pipeline" = {
      yml_path = "AzurePipelines/video-processor-application-pipeline.yml"
    }

    "video-store-api-pipeline" = {
      yml_path = "AzurePipelines/video-store-api-pipeline.yml"
    }

    "web-client-pipeline" = {
      yml_path = "AzurePipelines/web-client-pipeline.yml"
    }

    "web-status-pipeline" = {
      yml_path = "AzurePipelines/web-status-pipeline.yml"
    }
  }
}
