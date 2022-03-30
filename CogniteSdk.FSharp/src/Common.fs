namespace CogniteSdk.FSharp

open System

type Identifier =
    | Id of int64
    | ExternalId of string

    member x.ToIdentifier() =
        match x with
        | Id x -> CogniteSdk.Identity(x)
        | ExternalId x -> CogniteSdk.Identity(x)

// CDF cursor is either an opaque string or a time range
type Cursor =
    | StringCursor of string
    | DataPointAggregateCursor of nextPoints: seq<string * int64> * granularity: string

    member x.TryString() =
        match x with
        | StringCursor str -> Some str
        | _ -> None

    member x.TryDataPointAggregate() =
        match x with
        | DataPointAggregateCursor (nextPoints, granularity) -> Some(nextPoints, granularity)
        | _ -> None

    static member TryCreate(cursor: string) =
        match cursor with
        | ""
        | null -> None
        | value -> StringCursor value |> Some

    static member TryCreate(nextPoints: seq<string * int64>, granularity: string option) =
        match nextPoints, granularity with
        | points, _ when points |> Seq.isEmpty -> None
        | points, None -> None
        | points, Some granularity ->
            DataPointAggregateCursor(points, granularity)
            |> Some

type LabelFilter =
    | ContainsAny of string list
    | ContainsAll of string list

    member x.ToLabelFilter() =

        let filter =
            match x with
            | ContainsAny (xs) -> CogniteSdk.LabelFilter(CogniteSdk.LabelContainsAnyFilter(xs))
            | ContainsAll (xs) -> CogniteSdk.LabelFilter(CogniteSdk.LabelContainsAllFilter(xs))

        filter

type Search =
    { Name: string option
      Description: string option
      Query: string option }

    member x.ToSearch() =
        let search = CogniteSdk.Search()
        x.Name |> Option.iter (fun x -> search.Name <- x)

        x.Description
        |> Option.iter (fun x -> search.Description <- x)

        x.Query
        |> Option.iter (fun x -> search.Query <- x)

        search

type SearchDescription =
    { Description: string option }

    member x.ToSearch() =
        let search = CogniteSdk.DescriptionSearch()

        x.Description
        |> Option.iter (fun x -> search.Description <- x)

        search


type SearchName =
    { Name: string option }

    member x.ToSearch() =
        let search = CogniteSdk.NameSearch()

        x.Name |> Option.iter (fun x -> search.Name <- x)

        search

// Used by Assets, TimeSeries
type TimeRange =
    { Max: int64 option
      Min: int64 option }

    member x.ToTimeRange() =
        let tr = CogniteSdk.TimeRange()
        x.Max |> Option.iter (fun x -> tr.Max <- x)
        x.Min |> Option.iter (fun x -> tr.Min <- x)
        tr

    static member Create(lower, upper) =
        let min =
            lower
            |> Option.map (fun (dt: DateTimeOffset, inclusive) ->
                let ms = dt.ToUnixTimeMilliseconds()
                if inclusive then ms else ms + 1L)

        let max =
            upper
            |> Option.map (fun (dt: DateTimeOffset, inclusive) ->
                let ms = dt.ToUnixTimeMilliseconds()
                if inclusive then ms else ms - 1L)

        { Max = max; Min = min }

    /// pick least restrictive
    member x.Union(other) =
        let newMin =
            match x.Min, other.Min with
            | Some min1, Some min2 -> Some(min min1 min2)
            | None, _ -> None
            | _, None -> None

        let newMax =
            match x.Max, other.Max with
            | Some max1, Some max2 -> Some(max max1 max2)
            | None, _
            | _, None -> None

        { Min = newMin; Max = newMax }

    /// pick most restrictive
    member x.Intersect(other) =
        let newMin =
            match x.Min, other.Min with
            | None, None -> None
            | Some min1, Some min2 -> Some(max min1 min2)
            | Some x, None
            | None, Some x -> Some x

        let newMax =
            match x.Max, other.Max with
            | None, None -> None
            | Some max1, Some max2 -> Some(min max1 max2)
            | Some x, None
            | None, Some x -> Some x

        { Min = newMin; Max = newMax }
// Used by Events
[<RequireQualifiedAccess>]
type TimeFilter =
    | Range of TimeRange
    | IsNull

    member x.ToTimeRange() =
        match x with
        | Range (range) -> range.ToTimeRange()
        | IsNull ->
            let tr = CogniteSdk.TimeRange()
            tr.IsNull <- true
            tr

module Common =
    let emptyQuery<'T> : Oryx.HttpHandler<CogniteSdk.ItemsWithCursor<'T>> =
        Oryx.HttpHandler.singleton (CogniteSdk.ItemsWithCursor(Items = [], NextCursor = ""))

    let private maxTimestamp = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds()

    let fromCdfTimestamp (value: int64) =
        if value > maxTimestamp then
            DateTimeOffset.MaxValue
        else
            DateTimeOffset.FromUnixTimeMilliseconds value
