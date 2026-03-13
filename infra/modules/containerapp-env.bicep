@description('Location for the resource')
param location string

@description('Name of the Container Apps Environment')
param environmentName string

@description('Customer ID of the Log Analytics Workspace')
param logAnalyticsCustomerId string

@description('Primary shared key of the Log Analytics Workspace')
@secure()
param logAnalyticsPrimaryKey string

@description('Name of the storage account for Azure Files (SQL Server persistence)')
param storageAccountName string

@description('Name of the Azure Files share for SQL Server data')
param sqlFileShareName string = 'sqlserver-data'

@description('Tags to apply to the resource')
param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

resource fileService 'Microsoft.Storage/storageAccounts/fileServices@2023-05-01' = {
  parent: storageAccount
  name: 'default'
}

resource sqlFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-05-01' = {
  parent: fileService
  name: sqlFileShareName
  properties: {
    shareQuota: 100
  }
}

resource containerAppsEnv 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: environmentName
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsCustomerId
        sharedKey: logAnalyticsPrimaryKey
      }
    }
  }
}

// Attach the Azure Files storage to the Container Apps Environment for SQL Server persistence
resource sqlStorageLink 'Microsoft.App/managedEnvironments/storages@2024-03-01' = {
  parent: containerAppsEnv
  name: 'sqlserver-storage'
  properties: {
    azureFile: {
      accountName: storageAccount.name
      accountKey: storageAccount.listKeys().keys[0].value
      shareName: sqlFileShareName
      accessMode: 'ReadWrite'
    }
  }
}

@description('The resource ID of the Container Apps Environment')
output resourceId string = containerAppsEnv.id

@description('The name of the Container Apps Environment')
output name string = containerAppsEnv.name

@description('The default domain of the Container Apps Environment')
output defaultDomain string = containerAppsEnv.properties.defaultDomain

@description('The name of the SQL storage link (for volume mounts)')
output sqlStorageLinkName string = sqlStorageLink.name
