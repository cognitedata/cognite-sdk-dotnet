// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints

open System
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
            Average = if Double.IsNaN pt.Average then None else Some pt.Average
            Max = if Double.IsNaN pt.Max then None else Some pt.Max
            Min = if Double.IsNaN pt.Min then None else Some pt.Min
            Count = if Double.IsNaN pt.Count then None else Some (int pt.Count)
            Sum = if Double.IsNaN pt.Sum then None else Some pt.Sum
            Interpolation = if Double.IsNaN pt.Interpolation then None else Some pt.Interpolation
            StepInterpolation = if Double.IsNaN pt.StepInterpolation then None else Some pt.StepInterpolation
            ContinuousVariance = if Double.IsNaN pt.ContinuousVariance then None else Some pt.ContinuousVariance
            DiscreteVariance = if Double.IsNaN pt.DiscreteVariance then None else Some pt.DiscreteVariance
            TotalVariation = if Double.IsNaN pt.TotalVariation then None else Some pt.TotalVariation
        }

type ClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context
