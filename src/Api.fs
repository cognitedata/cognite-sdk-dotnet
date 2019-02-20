namespace Cognite.Sdk.Api

open System
open System.Threading.Tasks;

open Cognite.Sdk
open Cognite.Sdk.Context
open Cognite.Sdk.Assets


type AssetArgs (args: GetParams list) =
    let args  = args

    member this.Name(name: string) =
        let arg = Name name
        let newArgs = arg :: args
        AssetArgs(newArgs)


    member internal this.Args = args

    static member Empty() =
        AssetArgs([])


/// <summary>
/// Client for accessing the API.
/// </summary>
/// <param name="context">Context to use for this session.</param>
type Client private (context: Context) =
    let context = context

    new() = Client(defaultContext)

    member internal this.Ctx =
        context

    /// <summary>
    /// Add header for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.AddHeader(name: string, value: string)  =
        context
        |> addHeader (name, value)
        |> Client

    /// <summary>
    /// Set project for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.SetProject(project: string) =
        context
        |> setProject(project)
        |> Client

    /// <summary>
    /// Creates a Client for accessing the API.
    /// </summary>
    static member Create() =
        Client(defaultContext)

    /// <summary>
    /// Retrieve a list of all assets in the given project. The list is sorted alphabetically by name. This operation
    /// supports pagination.
    ///
    /// You can retrieve a subset of assets by supplying additional fields; Only assets satisfying all criteria will be
    /// returned. Names and descriptions are fuzzy searched using edit distance. The fuzziness parameter controls the
    /// maximum edit distance when considering matches for the name and description fields.
    /// </summary>
    /// <param name="args">The </param>
    member this.GetAssets (args: AssetArgs) : Task<AssetResponse> =
        let worker () : Async<AssetResponse> = async {
            let! result = getAssets context args.Args
            match result with
            | Ok response ->
                return response
            | Error ex -> 
                return raise ex
        }

        worker () |> Async.StartAsTask
