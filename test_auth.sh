#!/bin/sh
set -e
TENANTID=$TEST_TENANT_ID_WRITE
CLIENTID=$TEST_CLIENT_ID_WRITE
CLIENTSECRET=$TEST_CLIENT_SECRET_WRITE

TOKEN=$(curl -sX POST \
-F client_id="$CLIENTID" \
-F client_secret="$CLIENTSECRET" \
-F grant_type='client_credentials' \
-F scope='https://greenfield.cognitedata.com/.default' "https://login.microsoftonline.com/$TENANTID/oauth2/v2.0/token" | jq -j '.access_token')

export TEST_TOKEN_WRITE=$TOKEN
