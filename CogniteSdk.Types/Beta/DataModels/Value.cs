using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Linq;

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// Converter for DMSValue
    /// </summary>
    public class DmsValueConverter : JsonConverter<IDMSValue>
    {
        /// <inheritdoc />
        public override IDMSValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return new RawPropertyValue<double>(JsonSerializer.Deserialize<double>(ref reader, options));
            }
            else if (reader.TokenType == JsonTokenType.False || reader.TokenType == JsonTokenType.True)
            {
                return new RawPropertyValue<bool>(JsonSerializer.Deserialize<bool>(ref reader, options));
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return new RawPropertyValue<string>(JsonSerializer.Deserialize<string>(ref reader, options));
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var res = JsonSerializer.Deserialize<IEnumerable<IDMSValue>>(ref reader, options);
                // If the array is empty, we cannot determine the type.
                if (!res.Any())
                {
                    return new RawPropertyValue<object[]> { Value = new object[0] };
                }
                // Get the inner type of the value
                var types = res.Select(x => x.GetType().GenericTypeArguments[0]).Distinct();
                if (types.Count() > 1) throw new JsonException("Contents of DMS Value as array must all be same type");
                var type = types.First();

                // Create a RawPropertyValue<arrayType[]>
                var arrayType = type.MakeArrayType();
                var resultType = typeof(RawPropertyValue<>).MakeGenericType(arrayType);
                var result = Activator.CreateInstance(resultType);

                // Create an arrayType[]
                var resultArray = Array.CreateInstance(type, res.Count());
                // Convert the IDMSValue[] array into an object[] array using reflection
                var valueProp = res.First().GetType().GetProperty("Value");
                var valueArray = res.Select(r => valueProp.GetValue(r)).ToArray();

                Array.Copy(valueArray, resultArray, valueArray.Length);

                resultType.GetProperty("Value").SetValue(result, resultArray);
                return (IDMSValue)result;
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                // This can (unfortunately) be either an _arbitrary_ json object, or one of the special value types.
                // Since we really have no good way to distinguish between them, we just store it as JsonElement.
                // It is an edge case either way.
                var res = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
                return new RawPropertyValue<JsonElement>();
            }
            else
            {
                throw new JsonException("Unexpected filter value type");
            }

        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IDMSValue value, JsonSerializerOptions options)
        {
            if (value is IRawPropertyValue)
            {
                var innerValue = value.GetType().GetProperty("Value").GetValue(value);
                if (innerValue == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, innerValue, innerValue.GetType(), options);
                }
            }
            else
            {
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
            }
        }
    }

    /// <summary>
    /// Interface used for serializing the DMSValue types.
    /// </summary>
    public interface IDMSValue { }

    /// <summary>
    /// Non-generic base class of RawPropertyValue[T].
    /// </summary>
    public interface IRawPropertyValue : IDMSValue { }

    /// <summary>
    /// A value 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RawPropertyValue<T> : IRawPropertyValue
    {
        /// <summary>
        /// Value of this filter. Should be string, double, boolean, or an array of these.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public RawPropertyValue() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value, should be string, double, boolean, or an array of these</param>
        public RawPropertyValue(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// A parametrized FDM filter value.
    /// </summary>
    public class ParameterizedPropertyValue : IDMSValue
    {
        /// <summary>
        /// Parameter
        /// </summary>
        public string Parameter { get; set; }
    }

    /// <summary>
    /// A referenced FDM property value.
    /// </summary>
    public class ReferencedPropertyValue : IDMSValue
    {
        /// <summary>
        /// The referenced property.
        /// </summary>
        public IEnumerable<string> Property { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public ReferencedPropertyValue() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property">The referenced property</param>
        public ReferencedPropertyValue(params string[] property)
        {
            Property = property;
        }
    }
}
