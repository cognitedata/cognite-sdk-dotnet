# Cognite .NET SDK

An SDK for accessing the Cognite API (v5). Currently only works with F#
types, but a C# wrapper is also planned.

## C# API

The C# API is a fluent API using objects and method chaining and raises exceptions on errors.
The API is asynchronous so the methods are awaitabel and returns a `Task`.

## F# API

The F# API is written using plain asynchronous functions that returns `Async`.

## F# Core Domain

The SDK is written as a dual domain SDK for both F# and C#. F# is a succinct
and concise functional language that provides null safefy, correctness and 
completeness.

This reduces the need for unit-tests as incorrect state is much
harder to produce.

- F# Core Domain, domain modelled and exception free.
-
- Domain driven, improved modelling
- F# Core Domain, exception free, null safety, correctneess, completeness 
- Expressions at core
 
## Assets API

- getAsset
- getAssets
- deleteAssets
- updateAsset
- updateAssets

## Time Series API

- TBW