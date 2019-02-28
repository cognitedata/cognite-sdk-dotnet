FROM microsoft/dotnet:2.2-sdk AS build
COPY . .

RUN dotnet build