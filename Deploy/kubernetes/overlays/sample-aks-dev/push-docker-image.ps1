$tag = "1"
$name = "vspsample"
$registry = "${name}.azurecr.io"

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
  "web-status"
)

foreach ($image in $images) {
  $tagged_image = "${registry}/${image}:${tag}"
  docker tag $image $tagged_image
  docker push $tagged_image
}