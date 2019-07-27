#!/bin/sh

USERNAME=`xmllint --xpath "string(/configuration/packageSourceCredentials/CogniteV2/add[contains(@key,'Username')]/@value)" ./nuget.config`
PASSWORD=`xmllint --xpath "string(/configuration/packageSourceCredentials/CogniteV2/add[contains(@key,'ClearTextPassword')]/@value)" ./nuget.config`

mono .paket/paket.exe config add-credentials https://cognite.jfrog.io/cognite/api/nuget/nuget --username $USERNAME --password $PASSWORD
