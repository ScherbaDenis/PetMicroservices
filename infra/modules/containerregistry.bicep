@description('Location for the resource')
param location string

@description('Name of the Azure Container Registry')
param acrName string

@description('SKU for the ACR')
@allowed(['Basic', 'Standard', 'Premium'])
param sku string = 'Basic'

@description('Tags to apply to the resource')
param tags object = {}

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: acrName
  location: location
  tags: tags
  sku: {
    name: sku
  }
  properties: {
    adminUserEnabled: true
  }
}

@description('The login server URL of the ACR')
output loginServer string = acr.properties.loginServer

@description('The resource ID of the ACR')
output resourceId string = acr.id

@description('The name of the ACR')
output name string = acr.name
