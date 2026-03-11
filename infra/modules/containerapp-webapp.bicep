@description('Location for the resource')
param location string

@description('Name of the WebApp container app')
param appName string = 'webapp'

@description('Resource ID of the Container Apps Environment')
param containerAppsEnvId string

@description('Full container image reference')
param containerImage string

@description('ACR login server')
param acrLoginServer string

@description('ACR admin username')
param acrUsername string

@description('ACR admin password')
@secure()
param acrPassword string

@description('Base URL for the API gateway (e.g. https://api-gateway.xxx.azurecontainerapps.io)')
param apiGatewayUrl string

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

resource webApp 'Microsoft.App/containerApps@2024-03-01' = {
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
      ]
      registries: [
        {
          server: acrLoginServer
          username: acrUsername
          passwordSecretRef: 'acr-password'
        }
      ]
      // External ingress: publicly accessible
      ingress: {
        external: true
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
              name: 'ApiEndpoints__UserService'
              value: '${apiGatewayUrl}/user'
            }
            {
              name: 'ApiEndpoints__TopicService'
              value: '${apiGatewayUrl}/topic'
            }
            {
              name: 'ApiEndpoints__TemplateService'
              value: '${apiGatewayUrl}/template'
            }
            {
              name: 'ApiEndpoints__QuestionService'
              value: '${apiGatewayUrl}/question'
            }
            {
              name: 'ApiEndpoints__TagService'
              value: '${apiGatewayUrl}/tag'
            }
            {
              name: 'ApiEndpoints__CommentService'
              value: '${apiGatewayUrl}/comment'
            }
            {
              // Answer service is intentionally mounted at the gateway root
              // (matches docker-compose: ApiEndpoints__AnswerService=http://api-gateway:8080)
              name: 'ApiEndpoints__AnswerService'
              value: apiGatewayUrl
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

@description('The public FQDN of the WebApp container app')
output fqdn string = webApp.properties.configuration.ingress.fqdn

@description('The resource ID of the WebApp container app')
output resourceId string = webApp.id
