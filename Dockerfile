FROM eu.gcr.io/cognitedata/dotnet-mono:2.1-sdk AS build

WORKDIR /build
COPY . .

RUN mono .paket/paket.exe install
RUN dotnet build

WORKDIR /build/test/csharp
RUN dotnet test
WORKDIR /build/test/fsharp
RUN dotnet test