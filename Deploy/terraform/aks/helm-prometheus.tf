resource "helm_release" "prometheus_stack" {
  count = var.prometheus.install ? 1 : 0

  name       = "prometheus-stack"
  repository = "https://prometheus-community.github.io/helm-charts"
  chart      = "kube-prometheus-stack"

  create_namespace = true
  namespace        = var.prometheus.namespace

  version = var.prometheus.stack_version

  dynamic "set" {
    for_each = var.prometheus.stack_values

    content {
      name  = set.key
      value = set.value
    }
  }
}

resource "helm_release" "prometheus_adapter" {
  count = var.prometheus.install ? 1 : 0

  name       = "prometheus-adapter"
  repository = "https://prometheus-community.github.io/helm-charts"
  chart      = "prometheus-adapter"

  create_namespace = true
  namespace        = var.prometheus.namespace

  version = var.prometheus.adapter_version

  set {
    name  = "prometheus.url"
    value = "http://prometheus-stack-kube-prom-prometheus.${var.prometheus.namespace}.svc"
  }

  dynamic "set" {
    for_each = var.prometheus.adapter_values

    content {
      name  = set.key
      value = set.value
    }
  }

  depends_on = [
    helm_release.prometheus_stack
  ]
}
