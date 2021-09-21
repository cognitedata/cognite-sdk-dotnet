namespace Oryx.Cognite

open Google.Protobuf
open System.Net.Http
open System.Net.Http.Headers
open System.Threading.Tasks
open System.IO
open System.Net
open System.IO.Compression

[<AutoOpen>]
module GZip =
    type GZipProtobufStreamContent (content: IMessage, compression: CompressionLevel) =
        inherit HttpContent ()
        let _content = content
        let _compression = compression
        do base.Headers.ContentType <- MediaTypeHeaderValue "application/protobuf"
        do base.Headers.ContentEncoding.Add "gzip"

        override this.SerializeToStreamAsync(stream: Stream, context: TransportContext) : Task =
            use gzipStream = new GZipStream(stream, _compression, true)
            content.WriteTo gzipStream |> Task.FromResult :> _

        override this.TryComputeLength(length: byref<int64>) : bool =
            length <- -1L
            false
