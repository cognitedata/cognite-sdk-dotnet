namespace Cognite.Sdk.Timeseries

open Cognite.Sdk

type DataPointCreateDto = {
    TimeStamp: int64
    Value: Numeric
}

/// Id or ExternalId
type Identity =
    | Id of int64
    | ExternalId of string

type DataPointsCreateDto = {
    DataPoints: DataPointCreateDto seq
    Identity: Identity
}

type PointRequest = {
    Items: DataPointsCreateDto seq
}

type DataPointReadDto = {
    TimeStamp: int64
    Value: Numeric
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
}

type PointResponseDataPoints = {
    Id: int64
    ExternalId: string option
    IsString: bool
    DataPoints: DataPointReadDto seq
}

type PointResponseAggregateDataPoints = {
    Id: int64
    ExternalId: string option
    DataPoints: AggregateDataPointReadDto seq
}

type PointResponse = {
    Items: PointResponseDataPoints seq
}

type AggregatePointResponse = {
    Items: PointResponseAggregateDataPoints seq
}


type Item = {
    /// Id of item to retrieve
    Id: int64
}

/// Used for retrieving multiple time series
type TimeseriesReadRequest = {
    /// Sequence of items to retrieve
    Items: seq<Item>
}

type TimeseriesCreateDto = {
    ExternalId: string option
    /// Unique name of time series
    Name: string option
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
}

type TimeseriesRequest = {
    Items: seq<TimeseriesCreateDto>
}

type LatestDataRequest = {
    Before: string option
    Identity: Identity
}

type TimeseriesLatestRequest = {
    Items: seq<LatestDataRequest>
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
}

type TimeseriesResponse = {
    Items: TimeseriesReadDto seq
    NextCursor: string option
}

[<AutoOpen>]
module Model =
    [<Literal>]
    let Url = "/timeseries"

type Aggregate =
    | ContinuousVariance
    | StepInterpolation
    | DiscreteVariance
    | TotalVariation
    | Interpolation
    | Average
    | Count
    | Max
    | Min
    | Sum

    override this.ToString () =
        match this with
        | StepInterpolation -> "step"
        | ContinuousVariance -> "cv"
        | DiscreteVariance -> "dv"
        | Interpolation -> "int"
        | TotalVariation -> "tv"
        | Count -> "count"
        | Average -> "avg"
        | Max -> "max"
        | Min -> "min"
        | Sum -> "sum"

type Granularity =
    | Day of int
    | Hour of int
    | Minute of int
    | Second of int

/// Query parameters
type QueryDataParam =
    | Start of string
    | End of string
    | Aggregates of Aggregate seq
    | Granularity of Granularity
    | Limit of int32
    | IncludeOutsidePoints of bool

type QueryParam =
    | Limit of int32
    | IncludeMetaData of bool
    | Cursor of string
    | AssetIds of int64 seq

type QueryLatestParam =
    | Before of string
    | Id of int64
    | ExternalId of string