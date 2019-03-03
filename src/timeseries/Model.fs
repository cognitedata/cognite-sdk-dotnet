namespace Cognite.Sdk.Timeseries

type Numeric =
    | String of string
    | Integer of int64
    | Float of double

type DataPoint = {
    TimeStamp: int64
    Value: Numeric
}

type PointRequest = {
    Items: DataPoint list
}

type Timeseries = {
    Name: string
    Description: string
    IsString: bool
    MetaData: Map<string, string>
    Unit: string
    AssetId: int64
    IsStep: bool
    SecurityCategories: int64 list
}

type TimeseriesRequest = {
    Items: Timeseries list
}

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
    | Aggregates of Aggregate list
    | Granularity of Granularity
    | Limit of int32
    | IncludeOutsidePoints of bool
