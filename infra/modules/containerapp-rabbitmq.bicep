@description('Location for the resource')
param location string

@description('Name of the RabbitMQ container app')
param appName string = 'rabbitmq'

@description('Resource ID of the Container Apps Environment')
param containerAppsEnvId string

@description('RabbitMQ default user')
@secure()
param rabbitMqUser string

@description('RabbitMQ default password')
@secure()
param rabbitMqPass string

@description('Tags to apply to the resource')
param tags object = {}

resource rabbitMqApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  tags: tags
  properties: {
    environmentId: containerAppsEnvId
    configuration: {
      secrets: [
        {
          name: 'rabbitmq-user'
          value: rabbitMqUser
        }
        {
          name: 'rabbitmq-pass'
          value: rabbitMqPass
        }
      ]
      // Internal-only ingress on AMQP port 5672
      ingress: {
        external: false
        targetPort: 5672
        transport: 'tcp'
        exposedPort: 5672
      }
    }
    template: {
      containers: [
        {
          name: 'rabbitmq'
          image: 'rabbitmq:3-management'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          env: [
            {
              name: 'RABBITMQ_DEFAULT_USER'
              secretRef: 'rabbitmq-user'
            }
            {
              name: 'RABBITMQ_DEFAULT_PASS'
              secretRef: 'rabbitmq-pass'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

@description('The FQDN of the RabbitMQ container app (internal)')
output fqdn string = rabbitMqApp.properties.configuration.ingress.fqdn

@description('The resource ID of the RabbitMQ container app')
output resourceId string = rabbitMqApp.id
