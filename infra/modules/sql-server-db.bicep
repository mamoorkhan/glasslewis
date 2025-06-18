param environment string
param location string
param projectName string
param sqlServerName string

var sqlDatabaseName = 'sqldb-${projectName}-${environment}'

var dbSkuMap = {
  dev: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    capacity: 1
  }
  qa: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    capacity: 2
  }
  staging: {
    name: 'GP_Gen5'
    tier: 'GeneralPurpose'
    capacity: 2
  }
  preprod: {
    name: 'GP_Gen5'
    tier: 'GeneralPurpose'
    capacity: 4
  }
  prod: {
    name: 'GP_Gen5'
    tier: 'GeneralPurpose'
    capacity: 8
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' existing = {
  name: sqlServerName
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  name: sqlDatabaseName
  parent: sqlServer
  location: location
  sku: dbSkuMap[environment]
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 34359738368
    zoneRedundant: environment == 'prod'
    autoPauseDelay: environment == 'dev' ? 60 : null
    minCapacity: environment == 'dev' ? json('0.5') : null
    useFreeLimit: environment == 'dev'
    freeLimitExhaustionBehavior: 'BillOverUsage'
  }
}

output databaseName string = sqlDatabase.name
