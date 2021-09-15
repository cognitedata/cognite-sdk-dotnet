using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Raw table referenced from extraction pipeline
    /// </summary>
    public class ExtPipeRawTable
    {
        /// <summary>
        /// Database name
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// Table name
        /// </summary>
        public string TableName { get; set; }
    }
}
