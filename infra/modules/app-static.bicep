param environment string
param location string
param projectName string
param applicationInsightsInstrumentationKey string

// SKU based on environment
var skuName = environment == 'prod' ? 'Standard' : 'Free'

resource staticWebApp 'Microsoft.Web/staticSites@2023-12-01' = {
  name: 'swa-${projectName}-client-${environment}'
  location: location
  sku: {
    name: skuName
    tier: skuName
  }
  properties: {
    // Build configuration for Angular
    buildProperties: {
      appLocation: '/'
      outputLocation: 'dist'
      appBuildCommand: 'npm run build'
      apiBuildCommand: ''
      skipGithubActionWorkflowGeneration: true
    }
    // Custom domains and other settings
    allowConfigFileUpdates: true
    stagingEnvironmentPolicy: 'Enabled'
  }
  tags: {
    environment: environment
    project: projectName
  }
}

// Add Application Insights configuration
resource staticWebAppConfig 'Microsoft.Web/staticSites/config@2023-12-01' = {
  name: 'appsettings'
  parent: staticWebApp
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsightsInstrumentationKey
    APPLICATIONINSIGHTS_CONNECTION_STRING: 'InstrumentationKey=${applicationInsightsInstrumentationKey}'
  }
}

output staticWebAppUrl string = 'https://${staticWebApp.properties.defaultHostname}'
output staticWebAppName string = staticWebApp.name
output staticWebAppId string = staticWebApp.id
