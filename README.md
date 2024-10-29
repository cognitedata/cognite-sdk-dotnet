<a href="https://cognite.com/">
    <img src="./cognite_logo.png" alt="Cognite logo" title="Cognite" align="right" height="80" />
</a>

# CogniteSdk for .NET

![Build and Test](https://github.com/cognitedata/cognite-sdk-dotnet/workflows/Build%20and%20Test/badge.svg)
[![codecov](https://codecov.io/gh/cognitedata/cognite-sdk-dotnet/branch/master/graph/badge.svg?token=da8aPB6l9U)](https://codecov.io/gh/cognitedata/cognite-sdk-dotnet)
[![Nuget](https://img.shields.io/nuget/vpre/CogniteSdk)](https://www.nuget.org/packages/CogniteSdk/)

CogniteSdk for .NET is a cross platform asynchronous SDK for accessing the [Cognite Data Fusion](https://docs.cognite.com/) [API (v1)](https://docs.cognite.com/api/v1/) using [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) that works for all .NET implementations i.e both [.NET Core](https://en.wikipedia.org/wiki/.NET_Core) and [.NET Framework](https://en.wikipedia.org/wiki/.NET_Framework).

**Unofficial**: please note that this is an unofficial and community driven SDK. Feel free to open issues, or provide PRs if you want to improve the library.

The SDK may be used from both C# and F#.

- **C# SDK**: The C# SDK is a fluent API using objects and method chaining. Errors will be raised as exceptions. The API is asynchronous and all API methods returns `Task` and is awaitable using `async/await`.

- **F# SDK**: The F# API is written using plain asynchronous functions returning `Task` built on top of the [Oryx](https://github.com/cognitedata/oryx) HTTP handler library.

## Supported Resources
- Assets
- TimeSeries & DataPoints
- Events
- Files
- Raw
- Sequences
- Relationships
- Annotations
- 3D Models
- 3D Files
- 3D Asset Mapping
- Data sets
- Extractor Pipelines
- Transformations
- Labels
- Token
- Groups
- Units
- Data Models

### Beta Resources
- Subscriptions
- Templates

### Alpha Resources
- Simulators

## Documentation
* [Cognite API Documentation](https://api-docs.cognite.com/)
* [Cognite Developer Guide](https://developer.cognite.com/dev/)

## Installation

CogniteSdk is available as a [NuGet package](https://www.nuget.org/packages/CogniteSdk/). To install:

Using Package Manager:
```sh
Install-Package CogniteSdk
```

Using .NET CLI:
```sh
dotnet add package CogniteSdk
```

Or [directly in Visual Studio](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio).

## Quickstart

Requests to Cognite Data Fusion are authenticated as submitted by a client using OAuth2 tokens. There are several authentication flows available, check the [Cognite Documentation](https://docs.cognite.com/cdf/access/concepts/authentication_flows_oidc/#:~:text=In%20the%20client%20credentials%20grant,get%20a%20time%2Dlimited%20token.) for more details.

The SDK does not include any logic to fetch tokens from an Identity Provider. Instead, the SDK expects a valid token to be provided by the user. The SDK will then use this token to authenticate requests to CDF.

All SDK methods are called with a `Client` object. A valid client requires:
- `Project Name` - the name of your CDF project e.g `publicdata`.
- `App ID` - an identifier for your application. It is a free text string. Example: `asset-hierarchy-extractor`
- `Base URL` - the base URL for the CDF API. Example: `https://api.cognitedata.com`
- `Bearer Token` - valid OAuth2 token for the CDF project.
- `HTTP Client` - The [HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netcore-3.1) that will be used for the remote connection. Having this separate from the SDK have many benefits like using e.g [Polly](https://github.com/App-vNext/Polly) for policy handling.

Here's a simple example of how to instantiate a client. For this example we're using [Microsoft Authentication Library](https://www.nuget.org/packages/Microsoft.Identity.Client/) to fetch a token from Azure AD using client credentials flow, but any other method for fetching a token will work as well.
```c#
using CogniteSdk;
using Microsoft.Identity.Client;

var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
var cluster = Environment.GetEnvironmentVariable("CDF_CLUSTER");
var project = Environment.GetEnvironmentVariable("CDF_PROJECT");

var scopes = new List<string>{ $"https://{cluster}.cognitedata.com/.default" };

var app = ConfidentialClientApplicationBuilder
    .Create(clientId)
    .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
    .WithClientSecret(clientSecret)
    .Build();

var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
var accessToken = result.AccessToken;

var httpClient = new HttpClient();
var client = Client.Builder.Create(httpClient)
    .SetAppId("dotnet-sdk-client")
    .AddHeader("Authorization", $"Bearer {accessToken}")
    .SetProject(project)
    .SetBaseUrl(new Uri($"https://{cluster}.cognitedata.com"))
    .Build();

// your logic using the client
var query = new Assets.AssetQuery
{
    Filter = new Assets.AssetFilter { Name = assetName }
};
var result = await client.Assets.ListAsync(query);
```

> **_NOTE:_** The example above does not handle token refresh. If you need to refresh tokens, you need to implement this yourself or use a library like [Cognite Extractor Utils](https://github.com/cognitedata/dotnet-extractor-utils) that handles this for you.

## Examples

There are examples for both C# and F# in the Examples folder.

## Developing

### Dotnet Tools

A dotnet tools manifest is used to version tools used by this repo.  Install these tools with:

```sh
dotnet tool restore
```

This will install Paket locally which is used for dependency management.

### Dependencies

Dependencies for all projects are handled using [Paket](https://fsprojects.github.io/Paket/). To install dependencies:

```sh
dotnet paket install
```

This will install the main dependencies and sub-dependencies. The main dependencies are:

- [Oryx](https://www.nuget.org/packages/Oryx/) - HTTP Handlers.
- [Oryx.Cognite](https://www.nuget.org/packages/Oryx.Cognite/) - Oryx HTTP Handlers for Cognite API.
- [Oryx.SystemTextJson](https://www.nuget.org/packages/Oryx.SystemTextJson/) - JSON handlers for Oryx
- [Oryx.Protobuf](https://www.nuget.org/packages/Oryx.Protobuf/) - Protobuf handlers for Oryx
- [CogniteSdk.Protobuf](https://www.nuget.org/packages/CogniteSdk.Protobuf/) - Protobuf definitions for Cognite API.
- [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) - for Json support.
- [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf) - for Protobuf support.

### Running tests locally
To run the tests locally, you can use the following script:
```sh
sh ./test.sh
```
For this script, the following AAD environment variables need to be defined:
- `TEST_TENANT_ID_WRITE`
- `TEST_CLIENT_ID_WRITE`
- `TEST_CLIENT_SECRET_WRITE`

You also need read credentials for the `publicdata` project:
- `TEST_TENANT_ID_READ`
- `TEST_CLIENT_ID_READ`
- `TEST_CLIENT_SECRET_READ`

# Code of Conduct

This project follows https://www.contributor-covenant.org, see our [Code of Conduct](https://github.com/cognitedata/cognite-sdk-dotnet/blob/master/CODE_OF_CONDUCT.md).

## License

Apache v2, see [LICENSE](https://github.com/cognitedata/cognite-sdk-dotnet/blob/master/LICENSE).
