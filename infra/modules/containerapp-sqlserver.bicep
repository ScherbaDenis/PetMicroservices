@description('Location for the resource')
param location string

@description('Name of the SQL Server container app')
param appName string = 'sqlserver'

@description('Resource ID of the Container Apps Environment')
param containerAppsEnvId string

@description('SQL Server SA password (stored as ACA secret)')
@secure()
param saPassword string

@description('Name of the storage link in the Container Apps Environment')
param storageLinkName string

@description('Tags to apply to the resource')
param tags object = {}

resource sqlServerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  tags: tags
  properties: {
    environmentId: containerAppsEnvId
    configuration: {
      secrets: [
        {
          name: 'sa-password'
          value: saPassword
        }
      ]
      // Internal-only: no ingress exposed publicly
      ingress: {
        external: false
        targetPort: 1433
        transport: 'tcp'
        exposedPort: 1433
      }
    }
    template: {
      containers: [
        {
          name: 'sqlserver'
          image: 'mcr.microsoft.com/mssql/server:2022-latest'
          resources: {
            cpu: json('1.0')
            memory: '2Gi'
          }
          env: [
            {
              name: 'ACCEPT_EULA'
              value: 'Y'
            }
            {
              name: 'SA_PASSWORD'
              secretRef: 'sa-password'
            }
            {
              name: 'MSSQL_PID'
              value: 'Express'
            }
          ]
          volumeMounts: [
            {
              volumeName: 'sqlserver-data'
              mountPath: '/var/opt/mssql'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
      volumes: [
        {
          name: 'sqlserver-data'
          storageType: 'AzureFile'
          storageName: storageLinkName
        }
      ]
    }
  }
}

@description('The FQDN of the SQL Server container app (internal)')
output fqdn string = sqlServerApp.properties.configuration.ingress.fqdn

@description('The resource ID of the SQL Server container app')
output resourceId string = sqlServerApp.id
