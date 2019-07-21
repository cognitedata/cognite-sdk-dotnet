namespace Fusion.Sequences

open Fusion.Common

type ValueType =
    | String
    | Double
    | Long

    override this.ToString () =
        match this with
            | String -> "STRING"
            | Double -> "DOUBLE"
            | Long -> "LONG"

type ColumnCreateDto = {
    Name: string option
    ExternalId: string option
    Description: string option
    ValueType: ValueType
}

type ColumnReadDto = {
    Id: int64
    Name: string option
    ExternalId: string option
    Description: string option
    ValueType: ValueType
    MetaData: Map<string, string>
}

type SequenceCreateDto = {
    Name: string option
    Description: string option
    AssetId: int64 option
    ExternalId: string option
    MetaData: Map<string, string>
    Columns: ColumnCreateDto seq
}

type SequenceReadDto = {
    Id: int64
    Name: string option
    Description: string option
    AssetId: int64 option
    ExternalId: string option
    MetaData: Map<string, string>
    Columns: ColumnReadDto seq

    CreatedTime: int64
    LastUpdatedTime: int64
}

type SequenceId =
    | Id of int64
    | ExternalId of string

type SequenceCreateRow = {
    RowNumber: int64
    Values: Numeric seq
}

type SequenceCreateData = {
    Columns: SequenceId seq
    Rows: SequenceCreateRow seq
}

type SequenceDataCreateRequest = {
    Items: SequenceCreateData seq
}

type SequenceDataRequestItem = {
    InclusiveFrom: int64
    ExclusiveTo: int64
    Limit: int option

    Columns: SequenceId seq
}

type SequenceDataRequest = {
    Items: SequenceDataRequestItem seq
}

type SequenceDataResponse = {
    Data: exn
}
