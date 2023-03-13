// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// JsonConverter for untagged unions. This will attempt to convert to each
    /// provided type in order, returning the first that succeeds, or failing if none do.
    /// </summary>
    /// <typeparam name="T">Base type to convert</typeparam>
    public class UntaggedUnionConverter<T> : JsonConverter<T> where T : class
    {
        private IEnumerable<Type> _types;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="types">
        /// Types to attempt, in order. All types must be subtypes of <typeparamref name="T"/>
        /// </param>
        protected UntaggedUnionConverter(IEnumerable<Type> types)
        {
            _types = types;
        }

        /// <inheritdoc />
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var document = JsonDocument.ParseValue(ref reader);
            foreach (var type in _types)
            {
                try
                {
                    return (T)document.Deserialize(type, options);
                }
                catch {}
            }
            return null;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
