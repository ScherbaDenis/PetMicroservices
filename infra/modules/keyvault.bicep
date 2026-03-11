@description('Location for the resource')
param location string

@description('Name of the Key Vault')
param keyVaultName string

@description('Object ID of the managed identity that will access secrets')
param managedIdentityPrincipalId string

@description('Tags to apply to the resource')
param tags object = {}

@description('SQL Server SA password')
@secure()
param saPassword string

@description('RabbitMQ default username')
param rabbitMqUser string

@description('RabbitMQ default password')
@secure()
param rabbitMqPass string

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
  }
}

// Grant the managed identity the "Key Vault Secrets User" role
resource kvSecretsUserRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, managedIdentityPrincipalId, '4633458b-17de-408a-b874-0445c86b69e6')
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '4633458b-17de-408a-b874-0445c86b69e6'
    )
    principalId: managedIdentityPrincipalId
    principalType: 'ServicePrincipal'
  }
}

resource saPasswordSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'SA-PASSWORD'
  properties: {
    value: saPassword
  }
}

resource rabbitMqUserSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'RABBITMQ-USER'
  properties: {
    value: rabbitMqUser
  }
}

resource rabbitMqPassSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'RABBITMQ-PASS'
  properties: {
    value: rabbitMqPass
  }
}

@description('The resource ID of the Key Vault')
output resourceId string = keyVault.id

@description('The URI of the Key Vault')
output vaultUri string = keyVault.properties.vaultUri

@description('Key Vault name')
output name string = keyVault.name

@description('Secret URI for SA password (reference ID, not the value)')
output sqlSecretUri string = saPasswordSecret.properties.secretUri

@description('Secret URI for RabbitMQ user (reference ID, not the value)')
output rabbitMqUserSecretId string = rabbitMqUserSecret.properties.secretUri

@description('Secret URI for RabbitMQ password (reference ID, not the value)')
output rabbitMqPassSecretId string = rabbitMqPassSecret.properties.secretUri
