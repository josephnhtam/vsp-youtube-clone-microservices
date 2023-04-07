@echo off
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
helm install prometheus-stack prometheus-community/kube-prometheus-stack --version 45.8.1 --create-namespace --namespace monitoring
helm install prometheus-adapter prometheus-community/prometheus-adapter --version 4.1.1 --create-namespace --namespace monitoring --set prometheus.url=http://prometheus-stack-kube-prom-prometheus.monitoring.svc
kubectl wait --for=condition=Ready pods -l  app.kubernetes.io/instance=prometheus-stack --timeout=300s -n monitoring