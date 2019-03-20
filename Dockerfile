FROM eu.gcr.io/cognitedata/dotnet-mono:2.1-sdk AS build

WORKDIR /build
COPY . .

RUN dotnet build

WORKDIR /build/test/csharp
RUN dotnet test
WORKDIR /build/test/fsharp
RUN dotnet run