param(
  [string] $imageTag = "1",
  [string] $containerRegistry = "vspsample.azurecr.io"
)

$images = @(
  "api-gateway",
  "community-api",
  "history-api",
  "identity-provider",
  "library-api",
  "search-api",
  "storage-api",
  "subscriptions-api",
  "users-api",
  "video-manager-api",
  "video-manager-signalrhub",
  "video-processor-application",
  "video-store-api",
  "web-status",
  "web-client"
)

$maxRetries = 3

foreach ($image in $images) {
    $tagged_image = "${containerRegistry}/${image}:${imageTag}"
  
    docker tag $image $tagged_image
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to tag image $image"
        break
    }
  
    $retryCount = 0
	$success = $true

    do {
        docker push $tagged_image
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to push image $tagged_image"
            $retryCount++
			$success = $false
        } else {
            Write-Host "Pushed image $tagged_image"
			$success = $true
        }
    } while (!$success -and $retryCount -le $maxRetries)
    
    if (!$success) {
        Write-Error "Maximum retry count exceeded for pushing image $tagged_image"
        break
    }
}