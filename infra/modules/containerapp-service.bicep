@description('Location for the resource')
param location string

@description('Name of the backend service container app')
param appName string

@description('Resource ID of the Container Apps Environment')
param containerAppsEnvId string

@description('Full container image reference (e.g. myacr.azurecr.io/comment-service:latest)')
param containerImage string

@description('ACR login server')
param acrLoginServer string

@description('ACR admin username')
param acrUsername string

@description('ACR admin password')
@secure()
param acrPassword string

@description('Full SQL Server connection string for this service')
@secure()
param sqlConnectionString string

@description('RabbitMQ app name (used as host)')
param rabbitMqAppName string = 'rabbitmq'

@description('RabbitMQ username')
param rabbitMqUser string

@description('RabbitMQ password')
@secure()
param rabbitMqPass string

@description('CPU allocation for the container')
param cpu string = '0.5'

@description('Memory allocation for the container')
param memory string = '1Gi'

@description('Minimum replicas')
param minReplicas int = 1

@description('Maximum replicas')
param maxReplicas int = 3

@description('Tags to apply to the resource')
param tags object = {}

resource serviceApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  tags: tags
  properties: {
    environmentId: containerAppsEnvId
    configuration: {
      secrets: [
        {
          name: 'acr-password'
          value: acrPassword
        }
        {
          name: 'sql-connection-string'
          value: sqlConnectionString
        }
        {
          name: 'rabbitmq-pass'
          value: rabbitMqPass
        }
      ]
      registries: [
        {
          server: acrLoginServer
          username: acrUsername
          passwordSecretRef: 'acr-password'
        }
      ]
      // Internal-only: not publicly accessible
      ingress: {
        external: false
        targetPort: 8080
        transport: 'http'
      }
    }
    template: {
      containers: [
        {
          name: appName
          image: containerImage
          resources: {
            cpu: json(cpu)
            memory: memory
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'ConnectionStrings__DefaultConnection'
              secretRef: 'sql-connection-string'
            }
            {
              name: 'RabbitMQ__Host'
              value: rabbitMqAppName
            }
            {
              name: 'RabbitMQ__Username'
              value: rabbitMqUser
            }
            {
              name: 'RabbitMQ__Password'
              secretRef: 'rabbitmq-pass'
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

@description('The FQDN of the service container app (internal)')
output fqdn string = serviceApp.properties.configuration.ingress.fqdn

@description('The resource ID of the service container app')
output resourceId string = serviceApp.id
