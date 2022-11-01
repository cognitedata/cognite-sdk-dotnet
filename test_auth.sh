#!/bin/sh
set -e

TOKEN_WRITE=$(curl -sX POST \
-F client_id="$TEST_CLIENT_ID_WRITE" \
-F client_secret="$TEST_CLIENT_SECRET_WRITE" \
-F grant_type='client_credentials' \
-F scope='https://greenfield.cognitedata.com/.default' "https://login.microsoftonline.com/$TEST_TENANT_ID_WRITE/oauth2/v2.0/token" | jq -j '.access_token')

TOKEN_READ=$(curl -sX POST \
-F client_id="$TEST_CLIENT_ID_READ" \
-F client_secret="$TEST_CLIENT_SECRET_READ" \
-F grant_type='client_credentials' \
-F scope='https://api.cognitedata.com/.default' "https://login.microsoftonline.com/$TEST_TENANT_ID_READ/oauth2/v2.0/token" | jq -j '.access_token')

export TEST_TOKEN_READ=$TOKEN_READ
export TEST_TOKEN_WRITE=$TOKEN_WRITE