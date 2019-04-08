namespace Cognite.Sdk

/// Async extensions
module Async =

    let map f asyncX = async {
        let! x = asyncX
        return f x
    }

    let single x = async {
        return x
    }