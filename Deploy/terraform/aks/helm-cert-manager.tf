resource "helm_release" "cert_manager" {
  count = var.cert_manager.install ? 1 : 0

  name       = "cert-manager"
  repository = "https://charts.jetstack.io"
  chart      = "cert-manager"

  create_namespace = true
  namespace        = var.cert_manager.namespace

  version = var.cert_manager.version

  set {
    name  = "installCRDs"
    value = "true"
  }

  dynamic "set" {
    for_each = var.cert_manager.values

    content {
      name  = set.key
      value = set.value
    }
  }
}
