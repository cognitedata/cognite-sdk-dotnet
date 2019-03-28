[![build](https://webhooks.dev.cognite.ai/build/buildStatus/icon?job=github-builds/CogniteSdk.NET/master)](https://jenkins.cognite.ai/job/github-builds/job/CogniteSdk.NET/job/master/)
[![codecov](https://codecov.io/gh/cognitedata/CogniteSdk.NET/branch/master/graph/badge.svg)](https://codecov.io/gh/cognitedata/CogniteSdk.NET)

# Cognite .NET SDK

A cross platform and asynchronous SDK for accessing the Cognite API (v5) using [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard), thus it should work for all .NET implementations i.e both [.NET Core](https://en.wikipedia.org/wiki/.NET_Core) and [.NET Framework](https://en.wikipedia.org/wiki/.NET_Framework). Made as part of the 2019 February 14th hackathon.

The goal was to learn the Cognite API and experiment with [Domain Modeling](https://pragprog.com/book/swdddf/domain-modeling-made-functional) and Type Driven Development. Thus make the code reflect the specification as much as possible and hopefully be able to significantly reduce the need for unit-testing. That is also why I did't want to auto-generate the code using the OpenAPI tool chain.

The SDK is written as a dual domain SDK for both C# and F# with F# as the core domain model.

## Getting started

Download .NET Core (Mac/Linux/Windows) or .NET Framework (Windows) from https://dotnet.microsoft.com/download.

To build the sources on Linux/Mac:

```sh
> mono .paket/paket.exe install
> dotnet build
```

To build the sources on Windows:

```sh
> .paket\paket.exe install
> dotnet build
```

## Examples

 There are examples for both C# and F# in the Playground folder. To play with the example code, you need to set the CDP project and API key as environment variables in the shell.

```sh
export PROJECT=myprojet
export API_KEY=mysecretkey
```

## Dependencies

- [FSharp.Data](https://www.nuget.org/packages/FSharp.Data/) - for HTTP Utilities.
- [Thoth.Json.Net](https://www.nuget.org/packages/Thoth.Json.Net/2.5.0) - F# wrapper for Newtonsoft.Json.
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/12.0.1) - for JSON (de)serialization.

## C# API

The C# API is a fluent API using objects and method chaining that wraps the F# API. Errors will be
raised as exceptions. The API is asynchronous and all API methods returns `Task` and is thus awaitable using async/await.

## F# API

The F# API is written using plain asynchronous functions returning `Async`, and builds upon the core domain model.

## F# Core Domain

[F#](https://fsharp.org/) is a mature succinct and concise functional language that looks like Python but provides less magic, null safefy, correctness and completeness.

It allows us to [model our domain using types](https://fsharpforfunandprofit.com/ddd/). The code is also exception free and uses expressions at core. What this mean is that:

> If it compiles it's usually correct.

The goal is to reduce the need for unit-tests as incorrect state will be much harder to produce.

The C# API is also implemented in F# but hides F# specific datatypes such as `Options` and `Discriminated Unions`.

## Assets API

- `getAsset` - Get single asset.
- `getAssets` - Get multiple assets.
- `deleteAssets` - Delete one or more assets.
- `updateAsset` - Update single asset.
- `updateAssets` - Update one or more assets.

## Time Series API

- `insertDataByName` - Insert data into named time series.
- `createTimeSeries` - Create a new time series.
- `getTimeSeries` - Get timeseries with given id.
- `queryTimeSeries` - Retrieves a list of data points from a single time series.
- `deleteTimeseries`- Deletes a time series object given the name of the time series.