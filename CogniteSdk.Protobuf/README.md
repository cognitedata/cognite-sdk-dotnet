# CogniteSdk.Protobuf

Generated C# types for CDF time series datapoint protobuf payloads (`application/protobuf`).

## Regenerating from `.proto` sources

Proto definitions live in the separate [protobuf-files](https://github.com/cognitedata/protobuf-files) repository. Regenerate the three `.cs` files in this folder whenever those protos change.

From the repo root (adjust `PROTO_DIR`):

```bash
PROTO_DIR=/path/to/protobuf-files/v1/timeseries
OUT_DIR=$(mktemp -d)

protoc \
  --proto_path="$PROTO_DIR" \
  --csharp_out="$OUT_DIR" \
  --csharp_opt=file_extension=.cs,base_namespace=Com \
  "$PROTO_DIR/data_points.proto" \
  "$PROTO_DIR/data_point_insertion_request.proto" \
  "$PROTO_DIR/data_point_list_response.proto"

cp "$OUT_DIR/Cognite/V1/Timeseries/Proto/DataPoints.cs" ./DataPoints.cs
cp "$OUT_DIR/Cognite/V1/Timeseries/Proto/DataPointInsertionRequest.cs" ./DataPointInsertionRequest.cs
cp "$OUT_DIR/Cognite/V1/Timeseries/Proto/DataPointListResponse.cs" ./DataPointListResponse.cs
```

Then build and run tests. The generated files must **not** be edited by hand.

## Dependencies

This package targets **.NET Standard 2.0** and depends on **Google.Protobuf** (see `paket.references`).
