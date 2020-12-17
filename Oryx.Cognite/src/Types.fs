// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http
open System.Threading.Tasks

open Oryx

open CogniteSdk

// Shadow types for Oryx that pins the error type to ResponseError
type HttpFuncResult<'TResult> = Task<Result<Context<'TResult>, HandlerError<ResponseException>>>
type HttpFunc<'T, 'TResult> = Context<'T> -> HttpFuncResult<'TResult, ResponseException>
type NextFunc<'T, 'TResult> = HttpFunc<'T, 'TResult, ResponseException>
type public HttpHandler<'T, 'TNext, 'TResult> = HttpFunc<'TNext, 'TResult, ResponseException> -> Context<'T> -> HttpFuncResult<'TResult, ResponseException>
type HttpHandler<'T, 'TResult> = HttpHandler<'T, 'T, 'TResult, ResponseException>
type HttpHandler<'T> = HttpHandler<unit, 'T, ResponseException>
type HttpHandler = HttpHandler<unit, ResponseException>