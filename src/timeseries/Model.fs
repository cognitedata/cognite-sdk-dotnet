namespace Fusion.Timeseries

open System.Collections.Generic
open Com.Cognite.V1.Timeseries.Proto

type NumericDataPointDto = {
    TimeStamp: int64
    Value: float
} with
    static member FromProto (pt: NumericDatapoint) =
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
    static member FromProto (pt: AggregateDatapoint) =
        {
            TimeStamp = pt.Timestamp
            Average = Some pt.Average
            Max = Some pt.Max
            Min = Some pt.Min
            Count = Some (int pt.Count)
            Sum = Some pt.Sum
            Interpolation = Some pt.Interpolation
            StepInterpolation = Some pt.StepInterpolation
            ContinuousVariance = Some pt.ContinuousVariance
            DiscreteVariance = Some pt.DiscreteVariance
            TotalVariation = Some pt.TotalVariation
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
