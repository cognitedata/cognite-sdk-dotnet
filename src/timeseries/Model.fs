namespace CogniteSdk.TimeSeries

open System.Collections.Generic
open Com.Cognite.V1.Timeseries.Proto
open Oryx

/// Read/write timeseries type.
type TimeSeriesEntity internal (externalId: string, name: string, description: string, unit: string, isStep: bool, isString: bool, metaData: IDictionary<string, string>,securityCategories: int64 seq, id: int64, assetId: int64, legacyName: string, createdTime: int64, lastUpdatedTime: int64) =

    let mutable _legacyName : string = legacyName

    member val ExternalId : string = externalId with get, set
    member val Name : string = name with get, set
    member val Description : string = description with get, set
    member val IsStep : bool = isStep with get, set
    member val IsString : bool = isString with get, set
    member val Unit : string = unit with get, set

    member val MetaData : IDictionary<string, string> = metaData with get, set
    member val SecurityCategories : int64 seq = securityCategories with get, set
    member val Id : int64 = id with get
    member val AssetId: int64 = assetId with get, set
    member val CreatedTime : int64 = createdTime with get
    member val LastUpdatedTime : int64 = lastUpdatedTime with get

    member this.LegacyName
        with set (value) = _legacyName <- value
        and internal get () = _legacyName

    // Create new Asset.
    new () =
        TimeSeriesEntity(externalId=null, name=null, description=null, unit=null, isStep=false, isString=false, metaData=null, securityCategories=null, id=0L, assetId=0L, legacyName=null, createdTime=0L, lastUpdatedTime=0L)
    // Create new Asset.
    new (externalId: string, name: string, description: string, unit: string, isStep: bool, isString: bool, metaData: IDictionary<string, string>, securityCategories: int64 seq, assetId: int64, legacyName: string) =
        TimeSeriesEntity(externalId=externalId, name=name, description=description, unit=unit, isStep=isStep, isString=isString, metaData=metaData, securityCategories=securityCategories, id=0L, assetId=assetId, legacyName=legacyName, createdTime=0L, lastUpdatedTime=0L)

[<CLIMutable>]
type TimeSeriesItems = {
    Items: TimeSeriesEntity seq
    NextCursor: string
}

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

type TimeSeriesWriteDto = {
    /// Externally provided ID for the time series (optional, but recommended.)
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
    static member FromEntity (ts: TimeSeriesEntity) : TimeSeriesWriteDto =
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

type TimeSeriesReadDto = {
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
    member this.ToEntity () : TimeSeriesEntity =
        let name = if this.Name.IsSome then this.Name.Value else null
        let metaData = this.MetaData |> Map.toSeq |> dict
        let externalId = if this.ExternalId.IsSome then this.ExternalId.Value else null
        let description = if this.Description.IsSome then this.Description.Value else null
        let assetId = if this.AssetId.IsSome then this.AssetId.Value else Unchecked.defaultof<int64>
        let unit = if this.Unit.IsSome then this.Unit.Value else null
        let sc = if this.SecurityCategories.IsSome then this.SecurityCategories.Value else Seq.empty

        TimeSeriesEntity(
            id = this.Id,
            externalId = externalId,
            name = name,
            isString = this.IsString,
            metaData = metaData,
            unit = unit,
            assetId = assetId,
            isStep = this.IsStep,
            description = description,
            securityCategories = sc,
            createdTime = this.CreatedTime,
            lastUpdatedTime = this.LastUpdatedTime,
            legacyName = null
        )

type TimeSeriesClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context

type DataPointsClientExtension internal (context: HttpContext) =
    member internal __.Ctx =
        context
