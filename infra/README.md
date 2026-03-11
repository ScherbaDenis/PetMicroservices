# PetMicroservices – Azure Bicep IaC

This directory contains **Azure Bicep** Infrastructure-as-Code (IaC) templates
to provision the full PetMicroservices stack on **Azure Container Apps**.

---

## Architecture Overview

```
Azure Container Registry (ACR)
    └── Images: webapp, api-gateway, comment-service, template-service, answer-service

Log Analytics Workspace
Container Apps Environment
    ├── sqlserver        (internal TCP/1433 – Azure Files volume for /var/opt/mssql)
    ├── rabbitmq         (internal TCP/5672 – AMQP)
    ├── comment-service  (internal HTTP/8080)
    ├── template-service (internal HTTP/8080)
    ├── answer-service   (internal HTTP/8080)
    ├── api-gateway      (external HTTPS – public)
    └── webapp           (external HTTPS – public)
```

Secrets (SA password, RabbitMQ credentials) are stored directly as
**Azure Container Apps secrets** (no plain text in environment variables).

---

## Prerequisites

| Tool | Minimum version |
|------|-----------------|
| [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) | 2.55+ |
| Azure CLI extension `containerapp` | latest (`az extension add -n containerapp`) |
| Bicep CLI (bundled with Azure CLI) | 0.27+ |
| Docker (to build & push images) | 24+ |

You also need:

- An **Azure subscription**
- Contributor rights on the target resource group (or subscription)

---

## Quick Start

### 1. Login & set subscription

```bash
az login
az account set --subscription "<your-subscription-id>"
```

### 2. Create a resource group

```bash
az group create \
  --name rg-petms-dev \
  --location eastus
```

### 3. Build and push images to ACR

> **Important:** Run all `docker build` and `az deployment` commands from the
> **repository root** directory (the folder that contains `docker-compose.yml`).

First deploy just the ACR (or create it manually), then push images:

```bash
# Deploy ACR only (optional – main deployment also creates it)
az deployment group create \
  --resource-group rg-petms-dev \
  --template-file infra/main.bicep \
  --parameters infra/parameters/dev.bicepparam \
  --parameters saPassword="YourStrong!Passw0rd" rabbitMqPass="guest"

# Log in to ACR
ACR_NAME=$(az acr list --resource-group rg-petms-dev --query "[0].name" -o tsv)
az acr login --name $ACR_NAME

# Build and push each service image (run from the repository root)
docker build -t $ACR_NAME.azurecr.io/comment-service:latest  -f Comment/WebApiComment/Dockerfile  .
docker build -t $ACR_NAME.azurecr.io/template-service:latest -f Template/WebApiTemplate/Dockerfile .
docker build -t $ACR_NAME.azurecr.io/answer-service:latest   -f Answer/src/Answer.Api/Dockerfile   .
docker build -t $ACR_NAME.azurecr.io/api-gateway:latest      -f ApiGateway/ApiGateway/Dockerfile    .
docker build -t $ACR_NAME.azurecr.io/webapp:latest           -f BaseWebApplication/WebApp/Dockerfile .

docker push $ACR_NAME.azurecr.io/comment-service:latest
docker push $ACR_NAME.azurecr.io/template-service:latest
docker push $ACR_NAME.azurecr.io/answer-service:latest
docker push $ACR_NAME.azurecr.io/api-gateway:latest
docker push $ACR_NAME.azurecr.io/webapp:latest
```

### 4. Full deployment

```bash
az deployment group create \
  --resource-group rg-petms-dev \
  --template-file infra/main.bicep \
  --parameters infra/parameters/dev.bicepparam \
  --parameters saPassword="YourStrong!Passw0rd" \
               rabbitMqPass="guest"
```

> **Tip:** Pass secrets from environment variables to avoid shell history:
> ```bash
> az deployment group create \
>   --resource-group rg-petms-dev \
>   --template-file infra/main.bicep \
>   --parameters infra/parameters/dev.bicepparam \
>   --parameters saPassword="$SA_PASSWORD" \
>                rabbitMqPass="$RABBITMQ_PASS"
> ```

---

## Required Parameters

| Parameter | Description | Default | Required |
|-----------|-------------|---------|----------|
| `environmentName` | Short environment label (dev/staging/prod) | `dev` | No |
| `projectName` | Short project prefix for resource names | `petms` | No |
| `location` | Azure region | Resource group location | No |
| `saPassword` | SQL Server SA password (**secure**) | – | **Yes** |
| `rabbitMqUser` | RabbitMQ default username | `guest` | No |
| `rabbitMqPass` | RabbitMQ default password (**secure**) | – | **Yes** |
| `imageTag` | Container image tag to deploy | `latest` | No |

---

## Outputs

After a successful deployment the following outputs are printed:

| Output | Description |
|--------|-------------|
| `webAppUrl` | Public HTTPS URL of the WebApp |
| `apiGatewayUrl` | Public HTTPS URL of the API Gateway |
| `acrLoginServer` | ACR login server (for `docker push`) |
| `containerAppsEnvironmentName` | Container Apps Environment name |
| `containerAppsDefaultDomain` | Default domain for internal service DNS |

---

## Directory Structure

```
infra/
├── main.bicep                          # Entry point – orchestrates all modules
├── parameters/
│   └── dev.bicepparam                  # Dev environment parameters
├── modules/
│   ├── containerregistry.bicep         # Azure Container Registry
│   ├── loganalytics.bicep              # Log Analytics Workspace
│   ├── containerapp-env.bicep          # Container Apps Environment + Azure Files
│   ├── containerapp-sqlserver.bicep    # SQL Server container app (internal)
│   ├── containerapp-rabbitmq.bicep     # RabbitMQ container app (internal)
│   ├── containerapp-service.bicep      # Reusable module for backend services
│   ├── containerapp-gateway.bicep      # API Gateway (external)
│   └── containerapp-webapp.bicep       # WebApp (external)
└── README.md                           # This file
```

---

## Destroy / Clean Up

```bash
az group delete --name rg-petms-dev --yes --no-wait
```

---

## Notes

- **SQL Server in Container Apps** is functional but has operational trade-offs
  (persistence via Azure Files, no point-in-time restore, etc.).  
  Consider migrating to **Azure SQL Managed Instance** or **Azure SQL Database**
  for production workloads.
- RabbitMQ management UI (port 15672) is disabled in this setup.
  For monitoring, use the RabbitMQ CLI inside the container or enable it as a
  second internal ingress.
- The ACR admin credential is used for simplicity.  
  For production, switch to a **managed identity** pull credential.
