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
    Name: string
    DataPoints: DataPointReadDto seq
}

type PointResponseData = {
    Items: PointResponseDataPoints seq
}

type PointResponse = {
    Data: PointResponseData
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
    /// Unique name of time series
    Name: string
    /// Description of the time series.
    Description: string option
    /// Whether the time series is string valued or not.
    IsString: bool option
    /// Additional metadata. String key -> String value.
    MetaData: Map<string, string> option
    /// The physical unit of the time series.
    Unit: string option
    /// Asset that this time series belongs to.
    AssetId: int64 option
    /// Whether the time series is a step series or not.
    IsStep: bool option
    /// Security categories required in order to access this time series.
    SecurityCategories: seq<int64> option
    /// Time when this asset was created in CDF in milliseconds since Jan 1, 1970.
    CreatedTime: int64
    /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
    LastUpdatedTime: int64
}

type TimeseriesResponseData = {
    Items: TimeseriesReadDto seq
}

type TimeseriesResponse = {
    Data: TimeseriesResponseData
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
type QueryParams =
    | Start of int64
    | End of int64
    | Aggregates of Aggregate seq
    | Granularity of Granularity
    | Limit of int32
    | IncludeOutsidePoints of bool
