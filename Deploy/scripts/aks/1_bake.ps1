param(
  [string] $environment = "dev",
  [string] $domainName = "vspsample.online",
  [string] $containerRegistry = "vspsample.azurecr.io",
  [string] $imageTag = "1",
  [string] $imagePullSecret = "docker-config"
)

# ------------------------------------------------------------------- #

$tempFolderPath = "output"

# Check if output folder exists and remove if it does
if (Test-Path $tempFolderPath) {
  Remove-Item $tempFolderPath -Recurse -Force
}

New-Item -ItemType Directory -Path $tempFolderPath

# Generate Kubernetes manifests using kustomize and helm
Get-ChildItem -Path "../../kubernetes" -Recurse -Filter "kustomization.yml" | Where-Object { $_.DirectoryName -like "*\overlays\${environment}" } | ForEach-Object {
  $outputName = (Resolve-Path $_.FullName -Relative).Replace("\", "-").Replace("/", "-").Replace("..-", "").Replace(".-", "")
  kubectl kustomize $_.DirectoryName --enable-helm -o "${tempFolderPath}/${outputName}"
  if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to generate Kubernetes manifests for $($_.FullName)"
  }
  else {
    Write-Host "Generated Kubernetes manifests for $($_.FullName)"
  }
}

$values = @(
  $domainName,
  $containerRegistry,
  $imageTag
  $imagePullSecret
)

$vars = @(
  "VAR_DOMAIN_NAME"
  "VAR_CONTAINER_REGISTRY"
  "VAR_IMAGE_TAG"
  "VAR_IMAGE_PULL_SECRET"
)

# Replace the placeholders in the generated kubernetes manifests
Get-ChildItem -Path $tempFolderPath -Recurse | ForEach-Object {
  if ($_.Name -match "\.yml$") {
    $updated = $false
    $content = Get-Content $_.FullName

    for ($i = 0; $i -lt $vars.Count; $i++) {
      $var = $vars[$i]
      $value = $values[$i]

      if ($content -match $var) {
        $content = $content -replace $var, $value
        $updated = $true
      }
    }

    if ($updated) {
      Set-Content $_.FullName -Value $content
    }
  }
}