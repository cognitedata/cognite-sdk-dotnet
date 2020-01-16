using System.Collections.Generic;

namespace CogniteSdk
{
    public interface IQueryParams
    {
        List<(string, string)> ToQueryParams();
    }
}