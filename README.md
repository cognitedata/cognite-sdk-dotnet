<a href="https://cognite.com/">
    <img src="./cognite_logo.png" alt="Cognite logo" title="Cognite" align="right" height="80" />
</a>

# CogniteSdk for .Net

[![Build Status](https://travis-ci.org/cognitedata/cognite-sdk-dotnet.svg?branch=master)](https://travis-ci.org/cognitedata/cognite-sdk-dotnet)
[![codecov](https://codecov.io/gh/cognitedata/cognite-sdk-dotnet/branch/master/graph/badge.svg?token=da8aPB6l9U)](https://codecov.io/gh/cognitedata/cognite-sdk-dotnet)
[![Nuget](https://img.shields.io/nuget/vpre/CogniteSdk)](https://www.nuget.org/packages/CogniteSdk/)

_**Under development**, not recommended for production use cases_

CogniteSdk for .NET is a cross platform asynchronous SDK for accessing the [Cognite Data Fusion](https://docs.cognite.com/) [API (v1)](https://docs.cognite.com/api/v1/) using [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) that works for all .NET implementations i.e both [.NET Core](https://en.wikipedia.org/wiki/.NET_Core) and [.NET Framework](https://en.wikipedia.org/wiki/.NET_Framework).

The SDK may be used from both C# and F#.

- **C# SDK**: The C# SDK is a fluent API using objects and method chaining. Errors will be raised as exceptions. The API is asynchronous and all API methods returns `Task` and is awaitable using `async/await`.

- **F# SDK**: The F# API is written using plain asynchronous functions returning `Task` built on top of the [Oryx](https://github.com/cognitedata/oryx) HTTP handler library.

## Supported Resources

- [Assets](https://docs.cognite.com/api/v1/#tag/Assets)
- [TimeSeries & DataPoints](https://docs.cognite.com/api/v1/#tag/Time-series)
- [Events](https://docs.cognite.com/api/v1/#tag/Events)
- [Files](https://docs.cognite.com/api/v1/#tag/Files)
- [Login](https://docs.cognite.com/api/v1/#tag/Login) (partial)
- [Raw](https://docs.cognite.com/api/v1/#tag/Raw)
- [Sequences](https://docs.cognite.com/api/v1/#tag/Sequences)

## Documentation
* SDK Documentation. TBW.
* [API Documentation](https://doc.cognitedata.com/)
* [API Guide](https://doc.cognitedata.com/guides/api-guide.html)

## Installation

CogniteSdk is available as a [NuGet package](https://www.nuget.org/packages/CogniteSdk/). To install:

Using Package Manager:
```sh
Install-Package CogniteSdk -IncludePrerelease
```

Using .NET CLI:
```sh
dotnet add package CogniteSdk -v 1.0.0-alpha-014
```

Or [directly in Visual Studio](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio).

## Quickstart

The SDK supports authentication through api-keys. The best way to use the SDK is by setting authentication values to environment values:

Using Windows Commands:
```cmd
setx PROJECT=myprojet
setx API_KEY=mysecretkey
```

Using Shell:
```sh
export PROJECT=myprojet
export API_KEY=mysecretkey
```

All SDK methods are called with a `Client` object. A valid client requires:
- `API Key` - key used for authentication with CDF.
- `Project Name` - the name of your CDF project e.g `publicdata`.
- `App ID` - an identifier for your application. It is a free text string. Example: `asset-hierarchy-extractor`
- `HTTP Client` - The [HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netcore-3.1) that will be used for the remote connection. Having this separete from the SDK have many benefits like using e.g [Polly](https://github.com/App-vNext/Polly) for policy handling.

```c#
using CogniteSdk;

var apiKey = Environment.GetEnvironmentVariable("API_KEY");
var project = Environment.GetEnvironmentVariable("PROJECT");

using var httpClient = new HttpClient(handler);
var builder = new Client.Builder();
var client =
    builder
        .SetAppId("playground")
        .SetHttpClient(httpClient)
        .SetApiKey(apiKey)
        .SetProject(project)
        .Build();

// your logic using the client
var query = new Assets.AssetQuery
{
    Filter = new Assets.AssetFilter { Name = assetName }
};
var result = await client.Assets.ListAsync(query);
```

## Examples

There are examples for both C# and F# in the Playground folder. To play with the example code, you need to set the CDF project and API key as environment variables.

## Dependencies

Dependencies for all projects are handled using [Paket](https://fsprojects.github.io/Paket/). To install dependencies:

```sh
> dotnet new tool-manifest
> dotnet tool install Paket
> dotnet paket install
```

This will install the main dependencies and sub-dependencies. The main dependencies are:

- [Oryx](https://www.nuget.org/packages/Oryx/) - HTTP Handlers.
- [Oryx.Cognite](https://www.nuget.org/packages/Oryx.Cognite/) - Oryx HTTP Handlers for Cognite API.
- [Oryx.SystemTextJson](https://www.nuget.org/packages/Oryx.SystemTextJson/) - JSON handlers for Oryx
- [Oryx.Protobuf](https://www.nuget.org/packages/Oryx.Protobuf/) - Protobuf handlers for Oryx
- [CogniteSdk.Protobuf](https://www.nuget.org/packages/CogniteSdk.Protobuf/) - Protobuf definitions for Cognite API.
- [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) - for Json support.
- [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf) - for Protobuf support.

# Code of Conduct

This project follows https://www.contributor-covenant.org, see our [Code of Conduct](https://github.com/cognitedata/cognite-sdk-dotnet/blob/master/CODE_OF_CONDUCT.md).

## License

Apache v2, see [LICENSE](https://github.com/cognitedata/cognite-sdk-dotnet/blob/master/LICENSE).
