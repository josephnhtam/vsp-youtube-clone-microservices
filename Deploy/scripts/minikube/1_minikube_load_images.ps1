$images = @(
  "web-status",
  "identity-provider",
  "api-gateway",
  "community-api",
  "history-api",
  "library-api",
  "search-api",
  "storage-api",
  "subscriptions-api",
  "users-api",
  "video-manager-api",
  "video-manager-signalrhub",
  "video-store-api",
  "video-processor-application"
)

foreach ($image in $images) {
  Write-Host "Loading image $image"
  minikube image load $image
  if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully loaded image $image"
  }
  else {
    Write-Error "Failed to load image $image"
    break
  }
}