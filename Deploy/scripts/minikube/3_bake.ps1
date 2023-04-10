$environment = "minikube"
$outputFolderPath = "output"

# Check if output folder exists and remove if it does
if (Test-Path $outputFolderPath) {
  Remove-Item $outputFolderPath -Recurse -Force
}

New-Item -ItemType Directory -Path $outputFolderPath

# Generate Kubernetes manifests using kustomize and helm
Get-ChildItem -Path "../../kubernetes" -Recurse -Filter "kustomization.yml" | Where-Object { $_.DirectoryName -like "*\overlays\${environment}" } | ForEach-Object {
  $outputName = (Resolve-Path $_.FullName -Relative).Replace("\", "-").Replace("/", "-").Replace("..-", "").Replace(".-", "")
  kubectl kustomize $_.DirectoryName --enable-helm -o "${outputFolderPath}/${outputName}"
  if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to generate Kubernetes manifests for $($_.FullName)"
  }
  else {
    Write-Host "Generated Kubernetes manifests for $($_.FullName)"
  }
}