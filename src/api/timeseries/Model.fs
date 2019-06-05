namespace Cognite.Sdk.Api

type DataPoint (timestamp: int64) =
    member val TimeStamp = timestamp with get,set

    static member Integer(timestamp, value) =
        DataPointInteger(timestamp, value)

    static member Float(timestamp, value) =
        DataPointFloat(timestamp, value)

    static member String(timestamp, value) =
        DataPointString(timestamp, value)

and DataPointInteger (timestamp: int64, value: int64) =
    inherit DataPoint (timestamp)
    member val Value = value with get,set

and DataPointFloat(timestamp: int64, value: float) =
    inherit DataPoint (timestamp)

    member val Value = value with get,set

and DataPointString (timestamp: int64, value: string) =
    inherit DataPoint (timestamp)

    member val Value = value with get,set


[<AllowNullLiteral>]
type Identity () =
    static member Id value =
        IdentityId value

    static member ExternalId value =
        IdentityExternalId value

and IdentityId (value : int64) =
    inherit Identity ()

    member val Value = value with get, set

and IdentityExternalId (value : string) =
    inherit Identity ()
    member val Value = value with get, set

[<AllowNullLiteral>]
type DataPoints () =
    member val Identity: Identity = null with get, set
    member val DataPoints: DataPoint seq = null with get, set
