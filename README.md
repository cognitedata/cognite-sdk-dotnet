# Cognite .NET SDK

An SDK for accessing the Cognite API (v5). Made as part of the 2019 February 14th hackathon.

> Work in Progress (WIP)

The goal was to learn the Cognite API and experiment with [Domian Modeling](https://pragprog.com/book/swdddf/domain-modeling-made-functional) and Type Driven Development. Thus make the code reflect the specification as much as possible and hopefully be able to significantly reduce the need for unit-testing. That is also why I did't want to auto-generate the code using the OpenAPI tool chain.

The SDK is written as a dual domain SDK for both C# and F# with F# as the core domain model.

## C# API

The C# API is a fluent API using objects and method chaining that wraps the F# API. Errors will be
raised as exceptions. The API is asynchronous and all API methods returnins `Task` and is thus awaitable using async/await.

The C# API is implemented in F# but hides F# specific datatypes such as
`Options` and `Discriminated Unions`.

## F# API

The F# API is written using plain asynchronous functions returning `Async`, and builds upon the core domain model.

## F# Core Domain

[F#](https://fsharp.org/) is a mature succinct and concise functional language that looks like Python but provides less magic, null safefy, correctness and completeness.

It allows us to [model our domain using types](https://fsharpforfunandprofit.com/ddd/). The code is also exception free and uses expressions at core. What this mean is that:

> If it compiles it's usually correct.

The goal is to reduces the need for unit-tests as incorrect state will be much harder to produce.

## Assets API

- `getAsset` - Get single asset.
- `getAssets` - Get multiple assets.
- `deleteAssets` - Delete one or more assets.
- `updateAsset` - Update single asset.
- `updateAssets` - Update one or more assets.

## Time Series API

- `insertDataByName` - Insert data into named time series.
- `createTimeSeries` - Create a new time series.
- `getTimeSeries` -
- `queryTimeSeries`