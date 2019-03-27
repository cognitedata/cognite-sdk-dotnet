using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Cognite.Sdk;
using Cognite.Sdk.Assets;
using Cognite.Sdk.Api;

namespace Tests
{
    public class Fetcher
    {
        private readonly HttpResponse _response;
        private Context _ctx = null;

        public Fetcher(HttpResponse response)
        {
            this._response = response;
        }

        public Context Ctx => _ctx;

        public Task<HttpResponse> Fetch(Context ctx)
        {
            _ctx = ctx;
            return Task.FromResult(_response);
        }

        public static Fetcher FromJson(int statusCode, string json)
        {
            var response = new HttpResponse(statusCode, json);
            return new Fetcher(response);
        }
    }
}