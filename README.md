# CogniteSdk for .Net

[![build](https://webhooks.dev.cognite.ai/build/buildStatus/icon?job=github-builds/cognite-sdk-dotnet/master)](https://jenkins.cognite.ai/job/github-builds/job/cognite-sdk-dotnet/job/master/)
[![codecov](https://codecov.io/gh/cognitedata/cognite-sdk-dotnet/branch/master/graph/badge.svg?token=da8aPB6l9U)](https://codecov.io/gh/cognitedata/Fusion.NET)

CogniteSdk for .NET is a cross platform asynchronous SDK for accessing the Cognite Data Fusion API (v1) using [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) that works for all .NET implementations i.e both [.NET Core](https://en.wikipedia.org/wiki/.NET_Core) and [.NET Framework](https://en.wikipedia.org/wiki/.NET_Framework).

The SDK is written as a dual SDK for both C# and F#.

- **C# SDK**: The C# SDK is a fluent API using objects and method chaining. Errors will be raised as exceptions. The API is asynchronous and all API methods returns `Task` and is awaitable using `async/await`.

- **F# SDK**: The F# API is written using plain asynchronous functions returning `Async` built on top of the [Oryx](https://github.com/cognitedata/oryx) HTTP handler library.

## Supported Resources

- Assets
- TimeSeries
- DataPoints
- Events

## Documentation
* SDK Documentation. TBW.
* [API Documentation](https://doc.cognitedata.com/)
* [API Guide](https://doc.cognitedata.com/guides/api-guide.html)

## Installation

CogniteSdk is available as a [NuGet package](https://www.nuget.org/packages/CogniteSdk/). To install:

Using Package Manager:
```sh
Install-Package CogniteSdk -Version 1.0.0
```

Using .NET CLI
```sh
dotnet add package CogniteSdk --version 1.0.0
```

## Examples

 There are examples for both C# and F# in the Playground folder. To play with the example code, you need to set the CDF project and API key as environment variables in the shell.

```sh
export PROJECT=myprojet
export API_KEY=mysecretkey
```

## Dependencies

Dependencies for all projects are handled using [Paket](https://fsprojects.github.io/Paket/). To install dependencies:

```sh
> mono .paket/paket.exe install
```

This will install the main dependencies and sub-dependencies. The main dependencies are:

- [Oryx](https://www.nuget.org/packages/Oryx/) - HTTP Handlers.
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/12.0.1) - for JSON (de)serialization.
- [Thoth.Json.Net](https://www.nuget.org/packages/Thoth.Json.Net/2.5.0) - F# wrapper for Newtonsoft.Json.
- [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf) - for Protobuf support

# Code of Conduct

This project follows https://www.contributor-covenant.org, see our [Code of Conduct](https://github.com/cognitedata/oryx/blob/master/CODE_OF_CONDUCT.md).

## License

Apache v2, see [LICENSE](https://github.com/cognitedata/oryx/blob/master/LICENSE).