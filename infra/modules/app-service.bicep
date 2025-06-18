param environment string
param location string
param projectName string
param appServicePlanId string
param serviceType string
param applicationInsightsInstrumentationKey string
@secure()
param sqlConnectionString string

// Build app settings array conditionally
var baseAppSettings = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: environment
  }
  {
    name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
    value: applicationInsightsInstrumentationKey
  }
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: 'InstrumentationKey=${applicationInsightsInstrumentationKey}'
  }
]

var nodeAppSettings = serviceType == 'client' ? [
  {
    name: 'WEBSITE_NODE_DEFAULT_VERSION'
    value: '22.x'
  }
  {
    name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
    value: 'false'
  }
] : []

var allAppSettings = concat(baseAppSettings, nodeAppSettings)

resource appService 'Microsoft.Web/sites@2023-12-01' = {
  name: 'app-${projectName}-${serviceType}-${environment}'
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    reserved: true
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: environment != 'dev'
      appSettings: allAppSettings
      connectionStrings: serviceType == 'api' ? [
        {
          name: 'DefaultConnection'
          connectionString: sqlConnectionString
          type: 'SQLAzure'
        }
      ] : []
    }
  }
}

// Output managed identity principal ID for Key Vault access if needed
output appUrl string = 'https://${appService.properties.defaultHostName}'
output appName string = appService.name
output principalId string = appService.identity.principalId
