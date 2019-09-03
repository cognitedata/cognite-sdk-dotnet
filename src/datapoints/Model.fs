// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints

open System
open System.Collections.Generic
open Com.Cognite.V1.Timeseries.Proto
open Oryx


type NumericDataPointDto = {
    TimeStamp: int64
    Value: float
} with
    static member FromProtobuf (pt: NumericDatapoint) =
        {
            TimeStamp = pt.Timestamp
            Value = pt.Value
        }
type StringDataPointDto = {
    TimeStamp: int64
    Value: string
} with
    static member FromProto (pt: StringDatapoint) =
        {
            TimeStamp = pt.Timestamp
            Value = pt.Value
        }

type DataPointSeq =
    | Numeric of NumericDataPointDto seq
    | String of StringDataPointDto seq

type AggregateDataPointReadDto = {
    TimeStamp: int64
    Average: float option
    Max: float option
    Min: float option
    Count: int option
    Sum: float option
    Interpolation: float option
    StepInterpolation: float option
    ContinuousVariance: float option
    DiscreteVariance: float option
    TotalVariation: float option
} with
    static member FromProtobuf (pt: AggregateDatapoint) =
        {
            TimeStamp = pt.Timestamp
            Average = Some pt.Average
            Max = Some pt.Max
            Min = Some pt.Min
            Count = if Double.IsNaN pt.Count then None else Some (int pt.Count)
            Sum = Some pt.Sum
            Interpolation = Some pt.Interpolation
            StepInterpolation = Some pt.StepInterpolation
            ContinuousVariance = Some pt.ContinuousVariance
            DiscreteVariance = Some pt.DiscreteVariance
            TotalVariation = Some pt.TotalVariation
        }

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context
