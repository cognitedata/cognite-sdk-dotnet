#!/bin/sh

TOKEN_WRITE=$(curl -sX POST \
-F client_id="$TEST_CLIENT_ID_WRITE" \
-F client_secret="$TEST_CLIENT_SECRET_WRITE" \
--fail \
-F grant_type='client_credentials' \
-F scope='https://azure-dev.cognitedata.com/.default' "https://login.microsoftonline.com/$TEST_TENANT_ID_WRITE/oauth2/v2.0/token" | jq -j '.access_token')

if [[ -z "$TOKEN_WRITE" ]]
then
  echo "Error: count not get token"
fi

export TEST_TOKEN_WRITE=$TOKEN_WRITE
