# Cognite .NET SDK

An SDK for accessing the Cognite API (v5). Made as part of the 2019 February 14th hackathon and is work-in-progress (WIP).

The goal was to learn the Cognite API and experiment with Domian Modelling and Type Driven Development. Thus make the code reflect the specification as much as possible and remove the need for unit-testing.

The SDK is written as a dual domain SDK for both F# and C#.

## C# API

The C# API is a fluent API using objects and method chaining that wraps the F# API. Errors will be
raised as exceptions. The API is asynchronous so API methods are awaitabel and returns `Task`.

## F# API

The F# API is written using plain asynchronous functions that returns `Async`, and builds upon the core domain model.

## F# Core Domain

F# is a succinct and concise functional language that looks like Python but provides null safefy, correctness and completeness.

If it compiles it's usually correct, and this reduces the need for unit-tests as incorrect state will be much harder to produce.

- Domain modelled using types.
- Exception free.
- Null safety.
- Correctness and completeness.
- Expressions at core.
 
## Assets API

- getAsset
- getAssets
- deleteAssets
- updateAsset
- updateAssets

## Time Series API

- insertData