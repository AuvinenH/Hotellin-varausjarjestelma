targetScope = 'resourceGroup'

@description('Deployment location for all resources.')
param location string = resourceGroup().location

@description('App Service Plan name.')
param appServicePlanName string

@description('App Service Plan SKU name (for example F1, B1, S1).')
param appServicePlanSkuName string = 'B1'

@description('App Service Plan SKU tier (for example Free, Basic, Standard).')
param appServicePlanSkuTier string = 'Basic'

@description('Web App name for ASP.NET Core API.')
param webAppName string

@description('Storage account name (3-24 lowercase letters and numbers).')
@minLength(3)
@maxLength(24)
param storageAccountName string

@description('Storage account SKU.')
@allowed([
	'Standard_LRS'
	'Standard_GRS'
	'Standard_RAGRS'
	'Standard_ZRS'
])
param storageAccountSku string = 'Standard_LRS'

@description('Optional frontend origin for strict CORS (without trailing slash). Leave empty to allow all origins.')
param frontendOrigin string = ''

@description('SQLite file path used by the API in App Service.')
param sqliteDataSource string = '%HOME%\\data\\hotel-lakeview.db'

@description('Blob container used for room images.')
param roomImagesContainerName string = 'roomimages'

@description('Public access level for room images container.')
@allowed([
	'None'
	'Blob'
	'Container'
])
param roomImagesPublicAccess string = 'Blob'

@description('Set true only when using paid plans that support Always On.')
param alwaysOn bool = false

@description('Tags applied to all resources.')
param tags object = {}

var baseAppSettings = {
	ASPNETCORE_ENVIRONMENT: 'Production'
	Storage__RoomImagesPath: 'uploads/rooms'
}

var corsAppSettings = empty(frontendOrigin)
	? {}
	: {
			Cors__AllowedOrigins__0: frontendOrigin
		}

var mergedAppSettings = union(baseAppSettings, corsAppSettings)

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
	name: appServicePlanName
	location: location
	tags: tags
	sku: {
		name: appServicePlanSkuName
		tier: appServicePlanSkuTier
	}
	kind: 'app'
	properties: {
		reserved: false
	}
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
	name: webAppName
	location: location
	tags: tags
	kind: 'app'
	properties: {
		serverFarmId: appServicePlan.id
		httpsOnly: true
		siteConfig: {
			minTlsVersion: '1.2'
			ftpsState: 'Disabled'
			alwaysOn: alwaysOn
		}
	}
}

resource webAppAppSettings 'Microsoft.Web/sites/config@2023-12-01' = {
	name: 'appsettings'
	parent: webApp
	properties: mergedAppSettings
}

resource webAppConnectionStrings 'Microsoft.Web/sites/config@2023-12-01' = {
	name: 'connectionstrings'
	parent: webApp
	properties: {
		DefaultConnection: {
			value: 'Data Source=${sqliteDataSource}'
			type: 'Custom'
		}
	}
}

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
	name: storageAccountName
	location: location
	tags: tags
	sku: {
		name: storageAccountSku
	}
	kind: 'StorageV2'
	properties: {
		accessTier: 'Hot'
		allowBlobPublicAccess: true
		supportsHttpsTrafficOnly: true
		minimumTlsVersion: 'TLS1_2'
		allowSharedKeyAccess: true
	}
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
	name: 'default'
	parent: storage
	properties: {
		deleteRetentionPolicy: {
			enabled: true
			days: 7
		}
	}
}

resource staticWebsite 'Microsoft.Storage/storageAccounts/blobServices/services@2023-05-01' = {
	name: 'default'
	parent: blobService
	properties: {
		staticWebsite: {
			enabled: true
			indexDocument: 'index.html'
			errorDocument404Path: 'index.html'
		}
	}
}

resource roomImagesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
	name: roomImagesContainerName
	parent: blobService
	properties: {
		publicAccess: roomImagesPublicAccess
	}
}

output apiDefaultHostName string = webApp.properties.defaultHostName
output apiBaseUrl string = 'https://${webApp.properties.defaultHostName}'
output frontendStaticWebsiteUrl string = storage.properties.primaryEndpoints.web
output roomImagesContainerUrl string = 'https://${storage.name}.blob.${environment().suffixes.storage}/${roomImagesContainerName}'
output roomImagesContainerResourceId string = roomImagesContainer.id
