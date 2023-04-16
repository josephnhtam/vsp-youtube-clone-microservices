variable "application_deployment_variables" {
  type = map(string)
  default = {
    "devKubernetesServiceConnection"     = "dev-kubernetes-service-connection"
    "devDockerRegistryServiceConnection" = "dev-docker-registry-service-connection"

    "devKustomizationOverlayPath" = "overlays/dev"
    "devDomainName"               = "vspsample.online"
    "devContainerRegistry"        = "vspsample.azurecr.io"
    "devImagePullSecret"          = "docker-config"
  }
}
