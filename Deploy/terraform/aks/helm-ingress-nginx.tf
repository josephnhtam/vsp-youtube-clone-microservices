resource "helm_release" "ingress_nginx" {
  count = var.ingress_nginx.install ? 1 : 0

  name       = "ingress-nginx"
  repository = "https://kubernetes.github.io/ingress-nginx"
  chart      = "ingress-nginx"

  create_namespace = true
  namespace        = var.ingress_nginx.namespace

  version = var.ingress_nginx.version

  dynamic "set" {
    for_each = var.ingress_nginx.values

    content {
      name  = set.key
      value = set.value
    }
  }
}
