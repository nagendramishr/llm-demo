targetScope = 'subscription'

@description('The name of the Resource Group')
param resourceGroupName string

@description('The location of the Resource Group')
param location string

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

output resourceGroupName string = resourceGroup.name
output resourceGroupLocation string = resourceGroup.location
