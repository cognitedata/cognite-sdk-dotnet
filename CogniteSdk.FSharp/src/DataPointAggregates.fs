namespace CogniteSdk.FSharp

open System
open System.Text.RegularExpressions
open Oryx.Cognite
open Common


module DataPointAggregateType =
    type AggregateType =
        | Average
        | Count
        | Sum
        | Max
        | Min
        | Interpolation
        | StepInterpolation
        | DiscreteVariance
        | ContinuousVariance
        | TotalVariation

        override x.ToString() =
            match x with
            | Average -> "average"
            | Count -> "count"
            | Sum -> "sum"
            | Max -> "max"
            | Min -> "min"
            | Interpolation -> "interpolation"
            | StepInterpolation -> "stepInterpolation"
            | DiscreteVariance -> "discreteVariance"
            | ContinuousVariance -> "continuousVariance"
            | TotalVariation -> "totalVariation"

    /// all known aggregate types
    let knownAggregates =
        [ Average
          Count
          Sum
          Max
          Min
          Interpolation
          StepInterpolation
          DiscreteVariance
          ContinuousVariance
          TotalVariation ]

open DataPointAggregateType

type DataPointAggregatesQueryItem =
    { Id: int64 option
      ExternalId: string option
      TargetUnit: string option
      TargetUnitSystem: string option
      Start: string option
      End: string option
      Limit: int option
      Aggregates: AggregateType list option
      Granularity: string option
      IncludeOutsidePoints: bool option }

    member x.ToDataPointsQueryItem() =
        let query = CogniteSdk.DataPointsQueryItem()

        x.Id |> Option.iter (fun internalId -> query.Id <- internalId)

        x.ExternalId |> Option.iter (fun externalId -> query.ExternalId <- externalId)

        x.TargetUnit |> Option.iter (fun targetUnit -> query.TargetUnit <- targetUnit)

        x.TargetUnitSystem
        |> Option.iter (fun targetUnitSystem -> query.TargetUnitSystem <- targetUnitSystem)

        x.Start |> Option.iter (fun start -> query.Start <- start)

        x.End |> Option.iter (fun end' -> query.End <- end')

        x.Limit |> Option.iter (fun limit -> query.Limit <- limit)

        x.Aggregates
        |> Option.iter (function
            | [] -> ()
            | agg -> query.Aggregates <- agg |> List.map (fun x -> x.ToString()))

        x.Granularity
        |> Option.iter (fun granularity -> query.Granularity <- granularity)

        x.IncludeOutsidePoints
        |> Option.iter (fun toInclude -> query.IncludeOutsidePoints <- toInclude)

        query

    static member Empty =
        { Id = None
          ExternalId = None
          TargetUnit = None
          TargetUnitSystem = None
          Start = None
          End = None
          Limit = None
          Aggregates = None
          Granularity = None
          IncludeOutsidePoints = None }

type DataPointAggregatesQuery =
    { Start: string option
      End: string option
      Granularity: string option
      Limit: int32 option
      Aggregates: AggregateType list
      IncludeOutsidePoints: bool option
      Items: DataPointAggregatesQueryItem list }

    member x.ToDataPointsQuery() =
        let query = CogniteSdk.DataPointsQuery()

        x.Start |> Option.iter (fun start -> query.Start <- start)

        x.End |> Option.iter (fun end' -> query.End <- end')

        x.Granularity |> Option.iter (fun g -> query.Granularity <- g)

        x.Limit |> Option.iter (fun limit -> query.Limit <- limit)

        query.Aggregates <-
            match x.Aggregates with
            | [] -> knownAggregates
            | _ -> x.Aggregates
            |> List.map (fun x -> x.ToString())

        x.IncludeOutsidePoints
        |> Option.iter (fun toInclude -> query.IncludeOutsidePoints <- toInclude)

        query.Items <- x.Items |> List.map (fun queryItem -> queryItem.ToDataPointsQueryItem())

        query

    static member Empty =
        { Items = []
          Start = None
          End = None
          Limit = None
          Aggregates = []
          Granularity = None
          IncludeOutsidePoints = None }

module DataPointAggregates =
    let list (query: DataPointAggregatesQuery) =
        match query.Limit with
        | _ -> query.ToDataPointsQuery() |> DataPoints.list

    module Helpers =
        open Com.Cognite.V1.Timeseries.Proto

        let granularityToTimeSpan (strOpt: string option) : TimeSpan option =
            /// Active pattern for parsing an integer.
            let (|ParseInteger|_|) (str: string) =
                match System.Int32.TryParse(str) with
                | true, intvalue -> Some(intvalue)
                | false, _ -> None

            let (|ParseRegex|_|) regex str =
                let m = Regex(regex).Match(str)

                if m.Success then
                    Some(List.tail [ for x in m.Groups -> x.Value ])
                else
                    None

            let timeSpanDays = float >> TimeSpan.FromDays >> Some
            let timeSpanHours = float >> TimeSpan.FromHours >> Some
            let timeSpanMinutes = float >> TimeSpan.FromMinutes >> Some
            let timeSpanSeconds = float >> TimeSpan.FromSeconds >> Some

            strOpt
            |> Option.bind (fun str ->
                match str with
                | ParseRegex "(\d{1,4})d" [ ParseInteger day ] -> timeSpanDays day
                | ParseRegex "^d$" [] -> timeSpanDays 1
                | ParseRegex "(\d{1,4})h" [ ParseInteger hour ] -> timeSpanHours hour
                | ParseRegex "^h$" [] -> timeSpanHours 1
                | ParseRegex "(\d{1,4})m" [ ParseInteger min ] -> timeSpanMinutes min
                | ParseRegex "^m$" [] -> timeSpanMinutes 1
                | ParseRegex "(\d{1,4})s" [ ParseInteger sec ] -> timeSpanSeconds sec
                | ParseRegex "^s$" [] -> timeSpanSeconds 1
                | _ -> None)

        let getAggregateResultPerTimeseries (aggregateResponse: DataPointListResponse) =
            aggregateResponse.Items
            |> Seq.choose (fun timeseriesResult ->
                match timeseriesResult.DatapointTypeCase with
                | DataPointListItem.DatapointTypeOneofCase.AggregateDatapoints -> Some timeseriesResult
                | _ -> None)

        /// Convert response to list of datapointAggregates
        /// returns a flattened sequence of all aggregate datapoints tupled with the corresponding timeseries id
        let convertResponse (dataPointAggregateResponse: DataPointListResponse) (granularity: string option) =

            let resultPerTimeseries =
                dataPointAggregateResponse |> getAggregateResultPerTimeseries

            resultPerTimeseries
            |> Seq.map (fun resultFromSingleTimeseries ->
                let aggregatePoints = resultFromSingleTimeseries.AggregateDatapoints.Datapoints

                aggregatePoints
                |> Seq.map (fun aggregatePoint ->
                    (resultFromSingleTimeseries.ExternalId,
                     aggregatePoint,
                     (granularity |> Option.defaultValue String.Empty))))

            |> Seq.collect id

        let getNextPointPerTimeseries
            (endTime: int64 option)
            (granularity: string option)
            (dataPointAggregateResponse: DataPointListResponse)
            =
            let granularityAsTimeSpan =
                granularityToTimeSpan granularity
                |> Option.defaultValue (TimeSpan.FromDays(1.0))

            dataPointAggregateResponse
            |> getAggregateResultPerTimeseries
            |> Seq.map (fun dpAggList ->
                // pick the last datapoint for each timeseries in the response
                (dpAggList.AggregateDatapoints.Datapoints |> Seq.tryLast, dpAggList.ExternalId))
            |> Seq.choose (function
                // filter out exhausted timeseries (without last point)
                | Some dpAgg, extId ->
                    Some(
                        extId,
                        (dpAgg.Timestamp |> fromCdfTimestamp)
                            .Add(granularityAsTimeSpan)
                            .ToUnixTimeMilliseconds()
                    )
                | _ -> None)
            |> Seq.choose (fun (extId, nextTimestamp) ->
                match extId, nextTimestamp, endTime with
                | extId, nextTimestamp, Some endTime when nextTimestamp < endTime -> Some(extId, nextTimestamp)
                | extId, nextTimestamp, None -> Some(extId, nextTimestamp)
                | _ -> None)



        let toDataPointsQueryItems (nextPoints: seq<string * int64>) =
            nextPoints
            |> Seq.map (fun (extId, timestamp) ->
                { DataPointAggregatesQueryItem.Empty with
                    Start = timestamp.ToString() |> Some
                    ExternalId = Some extId })
