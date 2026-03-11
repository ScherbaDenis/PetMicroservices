@description('Location for the resource')
param location string

@description('Name of the Log Analytics Workspace')
param workspaceName string

@description('Tags to apply to the resource')
param tags object = {}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: workspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

@description('The resource ID of the Log Analytics Workspace')
output resourceId string = logAnalytics.id

@description('The customer ID (workspace ID) of the Log Analytics Workspace')
output customerId string = logAnalytics.properties.customerId

@description('The primary shared key of the Log Analytics Workspace')
@secure()
output primarySharedKey string = logAnalytics.listKeys().primarySharedKey
