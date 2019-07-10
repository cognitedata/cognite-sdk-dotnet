namespace Cognite.Sdk.Timeseries

open System
open Cognite.Sdk
open System.Collections.Generic

[<CLIMutable>]
type DataPointPoco = {
    TimeStamp : int64
    Value : Numeric
}

[<CLIMutable>]
type DataPointsWritePoco = {
    Identity: Identity
    DataPoints: DataPointPoco seq
}

type DataPointDto = {
    TimeStamp: int64
    Value: Numeric
} with
    static member FromPoco (pt: DataPointPoco) =
        {
            TimeStamp = pt.TimeStamp
            Value = pt.Value
        }

[<CLIMutable>]
type AggregateDataPointReadPoco = {
    TimeStamp: int64
    Average: float
    Max: float
    Min: float
    Count: int
    Sum: float
    Interpolation: float
    StepInterpolation: float
    ContinousVariance: float
    DiscreteVariance: float
    TotalVariation: float
}

type AggregateDataPointReadDto = {
    TimeStamp: int64
    Average: float option
    Max: float option
    Min: float option
    Count: int option
    Sum: float option
    Interpolation: float option
    StepInterpolation: float option
    ContinousVariance: float option
    DiscreteVariance: float option
    TotalVariation: float option
} with
    member this.ToPoco() : AggregateDataPointReadPoco =
        let average = if this.Average.IsSome then this.Average.Value else 0.0
        let max = if this.Max.IsSome then this.Max.Value else 0.0
        let min = if this.Min.IsSome then this.Min.Value else 0.0
        let count = if this.Count.IsSome then this.Count.Value else 0
        let sum = if this.Sum.IsSome then this.Sum.Value else 0.0
        let interpolation = if this.Interpolation.IsSome then this.Interpolation.Value else 0.0
        let stepInterpolation = if this.StepInterpolation.IsSome then this.StepInterpolation.Value else 0.0
        let continousVariance = if this.ContinousVariance.IsSome then this.ContinousVariance.Value else 0.0
        let discreteVariance = if this.DiscreteVariance.IsSome then this.DiscreteVariance.Value else 0.0
        let totalVariation = if this.TotalVariation.IsSome then this.TotalVariation.Value else 0.0

        {
            TimeStamp = this.TimeStamp
            Average = average
            Max = max
            Min = min
            Count = count
            Sum = sum
            Interpolation = interpolation
            StepInterpolation = stepInterpolation
            ContinousVariance = continousVariance
            DiscreteVariance = discreteVariance
            TotalVariation = totalVariation
        }

// C# compatible Timeserie POCO
[<CLIMutable>]
type TimeseriesWritePoco = {
    ExternalId : string
    Name : string
    LegacyName : string
    Description : string
    IsString : bool
    MetaData : IDictionary<string, string>
    Unit : string
    AssetId : int64
    IsStep : bool
    SecurityCategories : int64 seq
}

type TimeseriesWriteDto = {
    ExternalId: string option
    /// Unique name of time series
    Name: string option
    /// Set a value for legacyName to allow applications using API v0.3, v04,
    /// v05, and v0.6 to access this time series. The legacy name is the
    /// human-readable name for the time series and is mapped to the name field
    /// used in API versions 0.3-0.6. The legacyName field value must be
    /// unique, and setting this value to an already existing value will return
    /// an error. We recommend that you set this field to the same value as
    /// externalId.
    LegacyName: string option
    /// Description of the time series.
    Description: string option
    /// Whether the time series is string valued or not.
    IsString: bool
    /// Additional metadata. String key -> String value.
    MetaData: Map<string, string>
    /// The physical unit of the time series.
    Unit: string option
    /// Asset that this time series belongs to.
    AssetId: int64 option
    /// Whether the time series is a step series or not.
    IsStep: bool
    /// Security categories required in order to access this time series.
    SecurityCategories: seq<int64>
} with
    static member FromPoco (ts: TimeseriesWritePoco) : TimeseriesWriteDto =
        let metaData =
            if not (isNull ts.MetaData) then
                ts.MetaData |> Seq.map (|KeyValue|) |> Map.ofSeq
            else
                Map.empty
        {
            ExternalId = if isNull ts.ExternalId then None else Some ts.ExternalId
            Name = if isNull ts.Name  then None else Some ts.Name
            LegacyName = if isNull ts.LegacyName then None else Some ts.LegacyName
            Description = if isNull ts.Description then None else Some ts.Description
            IsString = ts.IsString
            MetaData = metaData
            Unit = if isNull ts.Unit  then None else Some ts.Unit
            AssetId = if ts.AssetId = 0L then None else Some ts.AssetId
            IsStep = ts.IsStep
            SecurityCategories = if isNull ts.SecurityCategories then Seq.empty else ts.SecurityCategories
        }

[<CLIMutable>]
type TimeseriesReadPoco = {
    Id : int64
    ExternalId : string
    Name : string
    IsString : bool
    MetaData : IDictionary<string, string>
    Unit : string
    AssetId : int64
    IsStep : bool
    Description : string
    SecurityCategories : int64 seq
    CreatedTime : int64
    LastUpdatedTime : int64
}

type TimeseriesReadDto = {
    /// Javascript friendly internal ID given to the object.
    Id: int64
    /// Externally supplied id of the time series
    ExternalId: string option
    /// Name of time series
    Name: string option
    /// Whether the time series is string valued or not.
    IsString: bool
    /// Additional metadata. String key -> String value.
    MetaData: Map<string, string>
    /// The physical unit of the time series.
    Unit: string option
    /// Asset that this time series belongs to.
    AssetId: int64 option
    /// Whether the time series is a step series or not.
    IsStep: bool
    /// Description of the time series.
    Description: string option
    /// Security categories required in order to access this time series.
    SecurityCategories: seq<int64> option
    /// Time when this asset was created in CDF in milliseconds since Jan 1, 1970.
    CreatedTime: int64
    /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
    LastUpdatedTime: int64
} with
    member this.ToPoco () : TimeseriesReadPoco =
        let name = if this.Name.IsSome then this.Name.Value else null
        let metaData = this.MetaData |> Map.toSeq |> dict
        let externalId = if this.ExternalId.IsSome then this.ExternalId.Value else null
        let description = if this.Description.IsSome then this.Description.Value else null
        let assetId = if this.AssetId.IsSome then this.AssetId.Value else Unchecked.defaultof<int64>
        let unit = if this.Unit.IsSome then this.Unit.Value else null
        let sc = if this.SecurityCategories.IsSome then this.SecurityCategories.Value else Seq.empty

        {
            Id = this.Id
            ExternalId = externalId
            Name = name
            IsString = this.IsString
            MetaData = metaData
            Unit = unit
            AssetId = assetId
            IsStep = this.IsStep
            Description = description
            SecurityCategories = sc
            CreatedTime = this.CreatedTime
            LastUpdatedTime = this.LastUpdatedTime
        }
