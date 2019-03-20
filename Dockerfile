FROM eu.gcr.io/cognitedata/dotnet-mono:2.1-sdk AS build
COPY . .

RUN dotnet build