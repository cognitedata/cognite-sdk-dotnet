// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Basic ACL class with a list of actions
    /// </summary>
    public class BaseAcl
    {
        /// <summary>
        /// List of actions. Valid values depend on resource type
        /// </summary>
        public IEnumerable<string> Actions { get; set; }
        /// <summary>
        /// If set, user has unscoped access to the resource.
        /// </summary>
        public BaseScope All { get; set; }
    }

    /// <summary>
    /// ACL with IdScope
    /// </summary>
    public class IdScopeAcl : BaseAcl
    {
        /// <summary>
        /// Restrict access based on internal ids for some other resource.
        /// </summary>
        [JsonPropertyName("idscope")]
        public IdScope IdScope { get; set; }
    }

    /// <summary>
    /// ACL with current user scope.
    /// </summary>
    public class CurrentUserScopeAcl : BaseAcl
    {
        /// <summary>
        /// Restrict access to the current user
        /// </summary>
        [JsonPropertyName("currentuserscope")]
        public BaseScope CurrentUserScope { get; set; }
    }

    /// <summary>
    /// ACL with data set scope
    /// </summary>
    public class DataSetScopeAcl : BaseAcl
    {
        /// <summary>
        /// Restrict access based on list of data set ids.
        /// </summary>
        public IdScope DatasetScope { get; set; }
    }

    /// <summary>
    /// Empty object for scopes.
    /// All scope types must be subtypes of this.
    /// </summary>
    public class BaseScope
    {
    }

    /// <summary>
    /// Scope containing a list of ids to some other resource.
    /// </summary>
    public class IdScope : BaseScope
    {
        /// <summary>
        /// List of internal ids for some other resource.
        /// </summary>
        public IEnumerable<long> Ids { get; set; }
    }

    /// <summary>
    /// Scope containing a list of ids to some other resource, as strings.
    /// </summary>
    public class IdScopeString : BaseScope
    {
        /// <summary>
        /// List of internal ids for some other resource.
        /// </summary>
        public IEnumerable<string> Ids { get; set; }
    }

    /// <summary>
    /// Class wrapping tables in DbsToTables in the raw table scope
    /// </summary>
    public class RawTableScopeWrapper
    {
        /// <summary>
        /// List of tables for this database
        /// </summary>
        public IEnumerable<string> Tables { get; set; }
    }

    /// <summary>
    /// Scope for restricting access based on raw databases and tables.
    /// </summary>
    public class RawTableScope : BaseScope
    {
        /// <summary>
        /// Map from database to list of tables.
        /// </summary>
        public IDictionary<string, RawTableScopeWrapper> DbsToTables { get; set; }
    }

    /// <summary>
    /// Scope for restricting access based on asset root ids.
    /// </summary>
    public class RootIdsScope
    {
        /// <summary>
        /// List of root asset internal ids.
        /// </summary>
        public IEnumerable<long> RootIds { get; set; }
    }

    /// <summary>
    /// JsonConverter for generic Acl objects.
    /// </summary>
    public class AclConverter : JsonConverter<BaseAcl>
    {
        private static readonly IDictionary<string, Type> _aclTypes = new Dictionary<string, Type>
        {
            { "groupsAcl", typeof(GroupsAcl) },
            { "assetsAcl", typeof(AssetsAcl) },
            { "eventsAcl", typeof(EventsAcl) },
            { "filesAcl", typeof(FilesAcl) },
            { "usersAcl", typeof(UsersAcl) },
            { "projectsAcl", typeof(ProjectsAcl) },
            { "securityCategoriesAcl", typeof(SecurityCategoriesAcl) },
            { "rawAcl", typeof(RawAcl) },
            { "timeSeriesAcl", typeof(TimeSeriesAcl) },
            { "apikeysAcl", typeof(ApiKeysAcl) },
            { "threedAcl", typeof(ThreedAcl) },
            { "sequencesAcl", typeof(SequencesAcl) },
            { "labelsAcl", typeof(LabelsAcl) },
            { "analyticsAcl", typeof(AnalyticsAcl) },
            { "digitalTwinAcl", typeof(DigitalTwinAcl) },
            { "relationshipsAcl", typeof(RelationshipsAcl) },
            { "datasetsAcl", typeof(DatasetsAcl) },
            { "seismicAcl", typeof(SeismicAcl) },
            { "typesAcl", typeof(TypesAcl) },
            { "modelHostingAcl", typeof(ModelHostingAcl) },
            { "functionsAcl", typeof(FunctionsAcl) },
            { "extractionPipelinesAcl", typeof(ExtPipesAcl) },
            { "extractionRunsAcl", typeof(ExtPipeRunsAcl) }
        };

        private static readonly IDictionary<Type, string> _aclNames = new Dictionary<Type, string>();

        static AclConverter()
        {
            foreach (var kvp in _aclTypes)
            {
                _aclNames[kvp.Value] = kvp.Key;
            }
        }

        private string GetPropertyName(PropertyInfo prop, JsonSerializerOptions options)
        {
            var attr = prop.GetCustomAttribute<JsonPropertyNameAttribute>(inherit: false);
            if (attr != null)
            {
                return attr.Name;
            }
            
            if (options.PropertyNamingPolicy != null)
            {
                return options.PropertyNamingPolicy.ConvertName(prop.Name);
            }

            return prop.Name;
        }

        /// <summary>
        /// Deserialize an acl object. This removes one layer of nesting, for convenience.
        /// </summary>
        /// <param name="reader">JsonReader</param>
        /// <param name="typeToConvert">Type to convert</param>
        /// <param name="options">Json serializer options</param>
        /// <returns>Subtype of <see cref="BaseAcl"/></returns>
        /// <exception cref="JsonException">If the input is not a valid Acl object</exception>
        public override BaseAcl Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, must be StartObject");
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Missing required acl property, no properties in group object");
            }

            var aclName = reader.GetString();

            if (!aclName.EndsWith("acl", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new JsonException($"Missing required acl property, property {aclName} does not end with acl");
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Missing required acl property, expected object got {reader.TokenType}");
            }

            Type type;
            if (!_aclTypes.TryGetValue(aclName, out type))
            {
                type = typeof(BaseAcl);
            }
            var result = (BaseAcl)Activator.CreateInstance(type);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException($"Unexpected token in acl object: {reader.TokenType}");
                }

                var propertyName = reader.GetString();
                if (propertyName == "actions")
                {
                    result.Actions = JsonSerializer.Deserialize<IEnumerable<string>>(ref reader, options);
                }
                else if (propertyName == "scope")
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.StartObject) continue;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject) break;
                        if (reader.TokenType != JsonTokenType.PropertyName)
                        {
                            throw new JsonException($"Unexpected token in scope object: {reader.TokenType}");
                        }

                        var scopeName = reader.GetString();

                        bool propFound = false;

                        foreach (var prop in type
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => typeof(BaseScope).IsAssignableFrom(p.PropertyType))
                            .Where(p => p.CanWrite))
                        {
                            var propName = GetPropertyName(prop, options);
                            if (propName.Equals(scopeName, options.PropertyNameCaseInsensitive
                                ? StringComparison.InvariantCultureIgnoreCase
                                : StringComparison.InvariantCulture))
                            {
                                propFound = true;
                                prop.SetValue(result, JsonSerializer.Deserialize(ref reader, prop.PropertyType, options));
                                break;
                            }
                        }

                        if (!propFound)
                        {
                            // Unknown capability type, skip scope
                            // We need this to "skip" arbitrary objects.
                            int sObj = 0, sArr = 0;

                            do
                            {
                                if (!reader.Read()) break;

                                if (reader.TokenType == JsonTokenType.StartObject) sObj++;
                                if (reader.TokenType == JsonTokenType.EndObject) sObj--;
                                if (reader.TokenType == JsonTokenType.StartArray) sArr++;
                                if (reader.TokenType == JsonTokenType.EndArray) sArr--;

                            } while (sObj > 0 || sArr > 0);
                        }
                    }
                }
            }

            reader.Read();

            return result;
        }

        /// <summary>
        /// Serialize an acl object.
        /// </summary>
        /// <param name="writer">Writer to write to</param>
        /// <param name="value">Object to write</param>
        /// <param name="options">Json serializer options</param>
        public override void Write(Utf8JsonWriter writer, BaseAcl value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var type = value.GetType();
            var name = _aclNames[type];

            writer.WritePropertyName(name);

            writer.WriteStartObject();

            writer.WritePropertyName("actions");
            JsonSerializer.Serialize(writer, value.Actions, options);

            writer.WritePropertyName("scope");
            writer.WriteStartObject();

            var scopeProp = value.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => typeof(BaseScope).IsAssignableFrom(p.PropertyType))
                .Where(p => p.CanRead)
                .Where(p => p.GetValue(value) != null)
                .First();

            var scopeValue = scopeProp.GetValue(value);
            var propName = GetPropertyName(scopeProp, options);
            writer.WritePropertyName(propName);
            JsonSerializer.Serialize(writer, scopeValue, options);

            writer.WriteEndObject();

            writer.WriteEndObject();

            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// Acl for access to the groups resource. 
    /// </summary>
    public class GroupsAcl : CurrentUserScopeAcl { }
    
    /// <summary>
    /// Acl for access to the assets resource.
    /// </summary>
    public class AssetsAcl : DataSetScopeAcl { }

    /// <summary>
    /// Acl for access to the events resource.
    /// </summary>
    public class EventsAcl : DataSetScopeAcl { }

    /// <summary>
    /// Acl for access to the files resource.
    /// </summary>
    public class FilesAcl : DataSetScopeAcl { }

    /// <summary>
    /// Acl for access to the users resource.
    /// </summary>
    public class UsersAcl : CurrentUserScopeAcl { }

    /// <summary>
    /// Acl for access to the projects resource.
    /// </summary>
    public class ProjectsAcl : BaseAcl { }

    /// <summary>
    /// Acl for access to the security categories resource.
    /// </summary>
    public class SecurityCategoriesAcl : BaseAcl
    {
        /// <summary>
        /// Restrict access based on internal ids for some other resource.
        /// </summary>
        [JsonPropertyName("idscope")]
        public IdScopeString IdScope { get; set; }
    }

    /// <summary>
    /// Acl for access to the raw resource.
    /// </summary>
    public class RawAcl : BaseAcl
    {
        /// <summary>
        /// Collection of allowed databases with tables.
        /// </summary>
        public RawTableScope TableScope { get; set; }
    }

    /// <summary>
    /// Acl for access to the timeseries resource.
    /// </summary>
    public class TimeSeriesAcl : IdScopeAcl
    {
        /// <summary>
        /// Restrict access based on list of root asset ids for associated assets.
        /// </summary>
        public RootIdsScope AssetRootIdScope { get; set; }
        /// <summary>
        /// Restrict access based on datasets.
        /// </summary>
        public IdScope DatasetScope { get; set; }
    }

    /// <summary>
    /// Acl for access to the api keys resource.
    /// </summary>
    public class ApiKeysAcl : BaseAcl
    {
        /// <summary>
        /// Restrict access based on internal ids for some other resource.
        /// </summary>
        [JsonPropertyName("idscope")]
        public IdScopeString IdScope { get; set; }
    }

    /// <summary>
    /// Acl for access to the 3d resource.
    /// </summary>
    public class ThreedAcl : BaseAcl { }

    /// <summary>
    /// Acl for access to the sequences resource.
    /// </summary>
    public class SequencesAcl : DataSetScopeAcl { }

    /// <summary>
    /// Acl for access to the labels resource.
    /// </summary>
    public class LabelsAcl : DataSetScopeAcl { }

    /// <summary>
    /// Acl for access to the analytics resource.
    /// </summary>
    public class AnalyticsAcl : BaseAcl { }

    /// <summary>
    /// Acl for access to the digital twin resource.
    /// </summary>
    public class DigitalTwinAcl : BaseAcl { }

    /// <summary>
    /// Acl for access to the relationships resource.
    /// </summary>
    public class RelationshipsAcl : DataSetScopeAcl { }

    /// <summary>
    /// Acl for access to the datasets resource.
    /// </summary>
    public class DatasetsAcl : IdScopeAcl { }

    /// <summary>
    /// Acl for access to the seismic resource.
    /// </summary>
    public class SeismicAcl : BaseAcl { }

    /// <summary>
    /// Acl for access to the types resource.
    /// </summary>
    public class TypesAcl : BaseAcl { }

    /// <summary>
    /// Acl for access to the model hosting resource.
    /// </summary>
    public class ModelHostingAcl : BaseAcl { }

    /// <summary>
    /// Acl for access to the functions resource.
    /// </summary>
    public class FunctionsAcl : BaseAcl { }

    /// <summary>
    /// Acl for access to the extraction pipelines resource.
    /// </summary>
    public class ExtPipesAcl : IdScopeAcl
    {
        /// <summary>
        /// Restrict access based on list of data set ids.
        /// </summary>
        public IdScope DatasetScope { get; set; }
    }

    /// <summary>
    /// Acl for access to the extraction pipeline runs resource.
    /// </summary>
    public class ExtPipeRunsAcl : DataSetScopeAcl
    {
        /// <summary>
        /// Scope by list of extraction pipeline internal ids.
        /// </summary>
        [JsonPropertyName("extractionpipelinescope")]
        public IdScope ExtractionPipelineScope { get; set; }
    }
}
