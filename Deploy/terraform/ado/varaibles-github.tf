variable "github_personal_access_token" {
  type        = string
  description = "The GitHub personal access token"
  sensitive   = true
}

variable "github_repo" {
  type        = string
  description = "The GitHub repository. This will take the form of <GitHub Org>/<Repo Name>"
}

variable "github_repo_default_branch" {
  type        = string
  description = "The branch name for which builds are triggered"
  default     = "refs/heads/master"
}
