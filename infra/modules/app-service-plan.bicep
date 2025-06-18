param environment string
param location string
param projectName string

var skuMap = {
  dev: {
    name: 'B1'
    tier: 'Basic'
  }
  qa: {
    name: 'B1'
    tier: 'Basic'
  }
  staging: {
    name: 'S1'
    tier: 'Standard'
  }
  preprod: {
    name: 'S2'
    tier: 'Standard'
  }
  prod: {
    name: 'P1V3'
    tier: 'PremiumV3'
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: 'plan-${projectName}-${environment}'
  location: location
  sku: skuMap[environment]
  kind: 'linux'
  properties: {
    reserved: true
  }
}

output planId string = appServicePlan.id
output planName string = appServicePlan.name
