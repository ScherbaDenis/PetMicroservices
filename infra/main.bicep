@description('Azure region for all resources')
param location string = resourceGroup().location

@description('Environment name prefix (e.g. dev, staging, prod)')
@minLength(2)
@maxLength(8)
param environmentName string = 'dev'

@description('Project name prefix for resource naming')
@minLength(2)
@maxLength(10)
param projectName string = 'petms'

@description('SQL Server SA password')
@secure()
param saPassword string

@description('RabbitMQ default username')
param rabbitMqUser string = 'guest'

@description('RabbitMQ default password')
@secure()
param rabbitMqPass string

@description('Image tag to deploy (e.g. latest, v1.0.0)')
param imageTag string = 'latest'

// ---------------------------------------------------------------------------
// Name helpers
// ---------------------------------------------------------------------------
var prefix = '${projectName}-${environmentName}'
// Storage account names: lowercase alphanumeric, 3-24 chars
var safePrefix = replace(replace(toLower('${projectName}${environmentName}'), '-', ''), '_', '')
var acrName = '${replace(safePrefix, '-', '')}acr'
var logAnalyticsName = '${prefix}-logs'
var containerEnvName = '${prefix}-env'
var storageAccountName = take('${safePrefix}st${uniqueString(resourceGroup().id)}', 24)

// Container image references
var commentServiceImage  = '${acr.outputs.loginServer}/comment-service:${imageTag}'
var templateServiceImage = '${acr.outputs.loginServer}/template-service:${imageTag}'
var answerServiceImage   = '${acr.outputs.loginServer}/answer-service:${imageTag}'
var apiGatewayImage      = '${acr.outputs.loginServer}/api-gateway:${imageTag}'
var webAppImage          = '${acr.outputs.loginServer}/webapp:${imageTag}'

// Connection strings (built here so password is never in plain env vars)
var commentConnStr  = 'Server=sqlserver;Database=CommentDb;User Id=sa;Password=${saPassword};TrustServerCertificate=True'
var templateConnStr = 'Server=sqlserver;Database=TemplateDb;User Id=sa;Password=${saPassword};TrustServerCertificate=True'
var answerConnStr   = 'Server=sqlserver;Database=AnswerDb;User Id=sa;Password=${saPassword};TrustServerCertificate=True'

var tags = {
  project: projectName
  environment: environmentName
  managedBy: 'bicep'
}

// ---------------------------------------------------------------------------
// Azure Container Registry
// ---------------------------------------------------------------------------
module acr 'modules/containerregistry.bicep' = {
  name: 'acr-deploy'
  params: {
    location: location
    acrName: acrName
    sku: 'Basic'
    tags: tags
  }
}

// ---------------------------------------------------------------------------
// Log Analytics Workspace
// ---------------------------------------------------------------------------
module logAnalytics 'modules/loganalytics.bicep' = {
  name: 'loganalytics-deploy'
  params: {
    location: location
    workspaceName: logAnalyticsName
    tags: tags
  }
}

// ---------------------------------------------------------------------------
// Container Apps Environment (+ Azure Files for SQL Server)
// ---------------------------------------------------------------------------
module containerEnv 'modules/containerapp-env.bicep' = {
  name: 'containerenv-deploy'
  params: {
    location: location
    environmentName: containerEnvName
    logAnalyticsCustomerId: logAnalytics.outputs.customerId
    logAnalyticsPrimaryKey: logAnalytics.outputs.primarySharedKey
    storageAccountName: storageAccountName
    sqlFileShareName: 'sqlserver-data'
    tags: tags
  }
}

// ---------------------------------------------------------------------------
// SQL Server container app (internal)
// ---------------------------------------------------------------------------
module sqlServer 'modules/containerapp-sqlserver.bicep' = {
  name: 'sqlserver-deploy'
  params: {
    location: location
    appName: 'sqlserver'
    containerAppsEnvId: containerEnv.outputs.resourceId
    saPassword: saPassword
    storageLinkName: containerEnv.outputs.sqlStorageLinkName
    tags: tags
  }
}

// ---------------------------------------------------------------------------
// RabbitMQ container app (internal)
// ---------------------------------------------------------------------------
module rabbitMq 'modules/containerapp-rabbitmq.bicep' = {
  name: 'rabbitmq-deploy'
  params: {
    location: location
    appName: 'rabbitmq'
    containerAppsEnvId: containerEnv.outputs.resourceId
    rabbitMqUser: rabbitMqUser
    rabbitMqPass: rabbitMqPass
    tags: tags
  }
}

// ---------------------------------------------------------------------------
// Backend microservices (internal)
// ---------------------------------------------------------------------------
// The ACR resource ID is computed directly so listCredentials can evaluate it
// at deployment start (BCP181 requirement).
var acrResourceId = resourceId('Microsoft.ContainerRegistry/registries', acrName)

module commentService 'modules/containerapp-service.bicep' = {
  name: 'comment-service-deploy'
  params: {
    location: location
    appName: 'comment-service'
    containerAppsEnvId: containerEnv.outputs.resourceId
    containerImage: commentServiceImage
    acrLoginServer: acr.outputs.loginServer
    acrUsername: acr.outputs.name
    acrPassword: listCredentials(acrResourceId, '2023-07-01').passwords[0].value
    sqlConnectionString: commentConnStr
    rabbitMqAppName: 'rabbitmq'
    rabbitMqUser: rabbitMqUser
    rabbitMqPass: rabbitMqPass
    tags: tags
  }
  dependsOn: [sqlServer, rabbitMq]
}

module templateService 'modules/containerapp-service.bicep' = {
  name: 'template-service-deploy'
  params: {
    location: location
    appName: 'template-service'
    containerAppsEnvId: containerEnv.outputs.resourceId
    containerImage: templateServiceImage
    acrLoginServer: acr.outputs.loginServer
    acrUsername: acr.outputs.name
    acrPassword: listCredentials(acrResourceId, '2023-07-01').passwords[0].value
    sqlConnectionString: templateConnStr
    rabbitMqAppName: 'rabbitmq'
    rabbitMqUser: rabbitMqUser
    rabbitMqPass: rabbitMqPass
    tags: tags
  }
  dependsOn: [sqlServer, rabbitMq]
}

module answerService 'modules/containerapp-service.bicep' = {
  name: 'answer-service-deploy'
  params: {
    location: location
    appName: 'answer-service'
    containerAppsEnvId: containerEnv.outputs.resourceId
    containerImage: answerServiceImage
    acrLoginServer: acr.outputs.loginServer
    acrUsername: acr.outputs.name
    acrPassword: listCredentials(acrResourceId, '2023-07-01').passwords[0].value
    sqlConnectionString: answerConnStr
    rabbitMqAppName: 'rabbitmq'
    rabbitMqUser: rabbitMqUser
    rabbitMqPass: rabbitMqPass
    tags: tags
  }
  dependsOn: [sqlServer, rabbitMq]
}

// ---------------------------------------------------------------------------
// API Gateway (external / public)
// ---------------------------------------------------------------------------
module apiGateway 'modules/containerapp-gateway.bicep' = {
  name: 'api-gateway-deploy'
  params: {
    location: location
    appName: 'api-gateway'
    containerAppsEnvId: containerEnv.outputs.resourceId
    containerImage: apiGatewayImage
    acrLoginServer: acr.outputs.loginServer
    acrUsername: acr.outputs.name
    acrPassword: listCredentials(acrResourceId, '2023-07-01').passwords[0].value
    aspNetCoreEnvironment: 'Docker'
    tags: tags
  }
  dependsOn: [commentService, templateService, answerService]
}

// ---------------------------------------------------------------------------
// WebApp (external / public)
// ---------------------------------------------------------------------------
module webApp 'modules/containerapp-webapp.bicep' = {
  name: 'webapp-deploy'
  params: {
    location: location
    appName: 'webapp'
    containerAppsEnvId: containerEnv.outputs.resourceId
    containerImage: webAppImage
    acrLoginServer: acr.outputs.loginServer
    acrUsername: acr.outputs.name
    acrPassword: listCredentials(acrResourceId, '2023-07-01').passwords[0].value
    apiGatewayUrl: 'https://${apiGateway.outputs.fqdn}'
    tags: tags
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------
@description('URL of the public WebApp')
output webAppUrl string = 'https://${webApp.outputs.fqdn}'

@description('URL of the public API Gateway')
output apiGatewayUrl string = 'https://${apiGateway.outputs.fqdn}'

@description('Azure Container Registry login server')
output acrLoginServer string = acr.outputs.loginServer

@description('Container Apps Environment name')
output containerAppsEnvironmentName string = containerEnv.outputs.name

@description('Container Apps Environment default domain')
output containerAppsDefaultDomain string = containerEnv.outputs.defaultDomain
