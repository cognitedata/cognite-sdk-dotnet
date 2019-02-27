# Cognite .NET SDK

An SDK for accessing the Cognite API (v5). Made as part of the 2019 February 14th hackathon and is work-in-progress (WIP).

The goal was to learn the Cognite API and experiment with [Domian Modelling](https://pragprog.com/book/swdddf/domain-modeling-made-functional) and Type Driven Development. Thus make the code reflect the specification as much as possible and hopefully be able to significantly reduce the need for unit-testing.

The SDK is written as a dual domain SDK for both C# and F#.

## C# API

The C# API is a fluent API using objects and method chaining that wraps the F# API. Errors will be
raised as exceptions. The API is asynchronous so API methods returns `Task` and are awaitabel.

## F# API

The F# API is written using plain asynchronous functions that returns `Async`, and builds upon the core domain model.

## F# Core Domain

[F#](https://fsharp.org/) is a mature succinct and concise functional language that looks like Python but provides less magic, null safefy, correctness and completeness.

It allows us to [model our domain using types](https://fsharpforfunandprofit.com/ddd/). The code is also exception free and uses expressions at core. What this mean is that:

> If it compiles it's usually correct

This reduces the need for unit-tests as incorrect state will be much harder to produce.

## Assets API

- getAsset
- getAssets
- deleteAssets
- updateAsset
- updateAssets

## Time Series API

- insertData