FROM eu.gcr.io/cognitedata/dotnet-mono:2.1-sdk AS build

WORKDIR /build
COPY . .

RUN mono .paket/paket.exe install
RUN dotnet build

RUN ./test.sh
