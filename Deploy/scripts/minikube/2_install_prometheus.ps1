# Add Prometheus Community repository
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to add Prometheus Community repository"
}
else {
  Write-Host "Successfully added Prometheus Community repository"
}

# Update repositories
helm repo update
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to update repositories"
}
else {
  Write-Host "Successfully updated repositories"
}

# Install Prometheus Stack
helm install prometheus-stack prometheus-community/kube-prometheus-stack --version 45.8.1 --create-namespace --namespace monitoring
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to install Prometheus Stack"
}
else {
  Write-Host "Successfully installed Prometheus Stack"
}

# Install Prometheus Adapter
helm install prometheus-adapter prometheus-community/prometheus-adapter --version 4.1.1 --create-namespace --namespace monitoring --set prometheus.url=http://prometheus-stack-kube-prom-prometheus.monitoring.svc
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to install Prometheus Adapter"
}
else {
  Write-Host "Successfully installed Prometheus Adapter"
}

# Wait for Prometheus Stack pods to become ready
kubectl wait --for=condition=Ready pods -l app.kubernetes.io/instance=prometheus-stack --timeout=300s -n monitoring
if ($LASTEXITCODE -ne 0) {
  Write-Error "Failed to wait for Prometheus Stack pods to become ready"
}
else {
  Write-Host "Prometheus Stack pods are now ready"
}

