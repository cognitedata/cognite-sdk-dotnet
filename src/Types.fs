namespace Cognite.Sdk

//type Name = Name of string
//type Description = Description of string

type ParentId = ParentId of int64
type ParentName = ParentName of string
type ParentRefId = ParentRefId of string

type ParentRef =
    | ParentId
    | ParentName
    | ParentRefId
