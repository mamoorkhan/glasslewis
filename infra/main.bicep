param environment string
param location string
param projectName string

var baseString = uniqueString(resourceGroup().id, projectName, environment)
var sqlAdminPassword = 'Gl@${toUpper(substring(baseString, 0, 4))}${toLower(substring(baseString, 4, 4))}${substring(baseString, 8, 4)}^${toUpper(environment)}!'


// Deploy Log Analytics Workspace first (required by Application Insights)
module logAnalytics 'modules/log-analytics.bicep' = {
  name: 'logAnalytics'
  params: {
    environment: environment
    location: location
    projectName: projectName
  }
}

// Deploy Application Insights
module applicationInsights 'modules/application-insights.bicep' = {
  name: 'applicationInsights'
  params: {
    environment: environment
    location: location
    projectName: projectName
    logAnalyticsWorkspaceId: logAnalytics.outputs.workspaceId
  }
}

// Deploy App Service Plan
module appServicePlan 'modules/app-service-plan.bicep' = {
  name: 'appServicePlan'
  params: {
    environment: environment
    location: location
    projectName: projectName
  }
}

// Deploy SQL Server
module sqlServer 'modules/sql-server.bicep' = {
  name: 'sqlServer'
  params: {
    environment: environment
    location: location
    projectName: projectName
    administratorLogin: 'glasslewis-admin'
    administratorPassword: sqlAdminPassword
  }
}

// Deploy SQL Database
module sqlServerDb 'modules/sql-server-db.bicep' = {
  name: 'sqlServerDb'
  params: {
    environment: environment
    location: location
    projectName: projectName
    sqlServerName: sqlServer.outputs.serverName
  }
}

// Deploy API App Service
module apiAppService 'modules/app-service.bicep' = {
  name: 'apiAppService'
  params: {
    environment: environment
    location: location
    projectName: projectName
    serviceType: 'api'
    appServicePlanId: appServicePlan.outputs.planId
    sqlConnectionString: 'Server=tcp:${sqlServer.outputs.fullyQualifiedDomainName},1433;Initial Catalog=${sqlServerDb.outputs.databaseName};Persist Security Info=False;User ID=glasslewis-admin;Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
    applicationInsightsInstrumentationKey: applicationInsights.outputs.instrumentationKey
  }
}

// Deploy Static Web App for Angular Client
module staticWebApp 'modules/app-static.bicep' = {
  name: 'staticWebApp'
  params: {
    environment: environment
    location: 'westeurope'
    projectName: projectName
    applicationInsightsInstrumentationKey: applicationInsights.outputs.instrumentationKey
  }
}

// Outputs
output apiAppUrl string = apiAppService.outputs.appUrl
output clientAppUrl string = staticWebApp.outputs.staticWebAppUrl
output sqlServerName string = sqlServer.outputs.serverName
output sqlServerId string = sqlServer.outputs.serverId
output databaseName string = sqlServerDb.outputs.databaseName
output connectionString string = 'Server=tcp:${sqlServer.outputs.fullyQualifiedDomainName},1433;Initial Catalog=${sqlServerDb.outputs.databaseName};Persist Security Info=False;User ID=glasslewis-admin;Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
