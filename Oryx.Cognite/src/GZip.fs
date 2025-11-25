namespace Oryx.Cognite

open System.Net.Http
open System.Net.Http.Headers
open System.Threading.Tasks
open System.IO
open System.Net
open System.IO.Compression

open Google.Protobuf
open System.Text.Json

[<AutoOpen>]
module GZip =
    type GZipProtobufStreamContent(content: IMessage, compression: CompressionLevel) =
        inherit HttpContent()
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

    type GZipJsonStreamContent<'T>(content: 'T, compression: CompressionLevel, options: JsonSerializerOptions) =
        inherit HttpContent()
        let _content = content
        let _compression = compression
        let _options = options
        do base.Headers.ContentType <- MediaTypeHeaderValue "application/json"
        do base.Headers.ContentEncoding.Add "gzip"

        override this.SerializeToStreamAsync(stream: Stream, context: TransportContext) : Task =
            task {
                use gzipStream = new GZipStream(stream, _compression, true)
                do! JsonSerializer.SerializeAsync(gzipStream, _content, _options)
                do! gzipStream.FlushAsync()
            } :> _
        
        override this.TryComputeLength(length: byref<int64>) : bool =
            length <- -1L
            false
