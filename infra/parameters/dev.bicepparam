using '../main.bicep'

param environmentName = 'dev'
param projectName     = 'petms'

// location defaults to the resource group location
// param location = 'eastus'

// Override image tag if needed
// param imageTag = 'latest'

// Secrets – supply at deploy time via --parameters or Key Vault references.
// Do NOT commit real passwords here.
// Example CLI usage:
//   az deployment group create \
//     --resource-group rg-petms-dev \
//     --template-file infra/main.bicep \
//     --parameters infra/parameters/dev.bicepparam \
//     --parameters saPassword='<strong-password>' \
//                  rabbitMqPass='<strong-password>'
