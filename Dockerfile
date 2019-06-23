FROM eu.gcr.io/cognitedata/dotnet-mono:2.2-sdk

WORKDIR /build
COPY . .

RUN mono .paket/paket.exe install
RUN dotnet build

RUN ./test.sh
RUN ./deploy.sh
