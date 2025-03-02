# Deployment Instructions

## Prerequisites
- Install the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- Install the [Bicep CLI](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/install)

## Steps to Deploy

Run the following code on your laptop or in the Azure cloud shell:

### 1. Log in to Azure
First, log in to your Azure account using the Azure CLI:
```
az login
```

### 2. Define environment variables
Define the following environment variables that will be used by the script:

```
export LOCATION=eastus
export resourceGroupName=samslife
export appServiceName=samslife
export appServicePlanName=samslife-appserviceplan
```

### 3. Deploy the Resource Group
Deploy the `ResourceGroup.bicep` file at the subscription level to create the resource group:
```
az deployment sub create --location $LOCATION --template-file /workspaces/llm-demo/IAC/ResourceGroup.bicep --parameters resourceGroupName=$resourceGroupName location=$LOCATION
```

### 4. Deploy the App Service
Deploy the `AppService.bicep` file within the created resource group:
```
az deployment group create --resource-group $resourceGroupName --template-file /workspaces/llm-demo/IAC/AppService.bicep --parameters resourceGroupName=$resourceGroupName location=$LOCATION appServicePlanName=$appServicePlanName appServiceName=$appServiceName
```
### 5. Download the Publish Profile
Download the publish profile for the App Service. This profile will be used to deploy your application from GitHub Actions:
```
az webapp deployment list-publishing-profiles --name $appServiceName --resource-group $resourceGroupName --query "[?publishMethod=='MSDeploy'].{publishUrl:publishUrl, userName:userName, userPWD:userPWD}" --output tsv > publishProfile.txt
```

### 6. Upload the Publish Profile as a GitHub Secret
Go to your GitHub repository, navigate to **Settings** > **Secrets and variables** > **Actions** > **New repository secret**. Name the secret `AZURE_WEBAPP_PUBLISH_PROFILE` and upload the contents of the `publishProfile.txt` file.
