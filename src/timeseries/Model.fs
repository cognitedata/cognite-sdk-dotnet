namespace Cognite.Sdk.Timeseries

type Numeric =
    | String of string
    | Integer of int64
    | Float of double

type DataPointCreateDto = {
    TimeStamp: int64
    Value: Numeric
}

type PointRequest = {
    Items: DataPointCreateDto seq
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
type RetrieveRequest = {
    /// Sequence of items to retrieve
    Items: seq<Item>
}

type TimeseriesCreateDto = {
    /// Unique name of time series
    Name: string
    /// Description of the time series.
    Description: string option
    /// Whether the time series is string valued or not.
    IsString: bool option
    /// Additional metadata. String key -> String value.
    MetaData: Map<string, string>
    /// The physical unit of the time series.
    Unit: string option
    /// Asset that this time series belongs to.
    AssetId: int64 option
    /// Whether the time series is a step series or not.
    IsStep: bool option
    /// Security categories required in order to access this time series.
    SecurityCategories: seq<int64>
}

type TimeseriesRequest = {
    Items: seq<TimeseriesCreateDto>
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

    override this.ToString () =
        match this with
        | Day day when day = 1 -> "d"
        | Day day -> sprintf "d%d" day
        | Hour hour when hour = 1 -> "h"
        | Hour hour -> sprintf "h%d" hour
        | Minute min when min = 1 -> "m"
        | Minute min -> sprintf "m%d" min
        | Second sec when sec = 1 -> "s"
        | Second sec -> sprintf "s%d" sec

/// Query parameters
type QueryDataParams =
    | Start of string
    | End of string
    | Aggregates of Aggregate seq
    | Granularity of Granularity
    | Limit of int32
    | IncludeOutsidePoints of bool

type QueryParams =
    | Limit of int32
    | IncludeMetaData of bool
    | Cursor of string
    | AssetIds of int64 seq
