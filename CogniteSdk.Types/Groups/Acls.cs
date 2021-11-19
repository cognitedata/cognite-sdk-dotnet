// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
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
    }

    /// <summary>
    /// Base generic ACL class with scope and list of actions.
    /// </summary>
    /// <typeparam name="TScope">Type of scope</typeparam>
    public class BaseAcl<TScope> : BaseAcl where TScope : BaseScope
    {
        /// <summary>
        /// Scope for the resource Acl.
        /// </summary>
        public TScope Scope { get; set; }
    }

    /// <summary>
    /// Empty object for scopes.
    /// </summary>
    public class EmptyScope
    {
    }

    /// <summary>
    /// Base scope class, with all scope, which is common to all resource types.
    /// </summary>
    public class BaseScope
    {
        /// <summary>
        /// If set, user has unscoped access to the resource.
        /// </summary>
        public EmptyScope All { get; set; }
    }

    /// <summary>
    /// Scope containing a list of ids to some other resource.
    /// </summary>
    public class IdScope
    {
        /// <summary>
        /// List of internal ids for some other resource.
        /// </summary>
        public IEnumerable<long> Ids { get; set; }
    }

    /// <summary>
    /// Scope containing a list of ids to some other resource, as strings.
    /// </summary>
    public class IdScopeString
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
    public class RawTableScope
    {
        /// <summary>
        /// Map from database to list of tables.
        /// </summary>
        public IDictionary<string, RawTableScopeWrapper> DbsToTables { get; set; }
    }
    /// <summary>
    /// Scope for restricting access based on raw databases and tables.
    /// </summary>
    public class WithRawTableScope : BaseScope
    {
        /// <summary>
        /// Collection of allowed databases with tables.
        /// </summary>
        public RawTableScope TableScope { get; set; }
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
    /// Scope containing "idscope", restricting access based on internalIds for some other resource.
    /// </summary>
    public class WithIdScope : BaseScope
    {
        /// <summary>
        /// Restrict access based on internal ids for some other resource.
        /// </summary>
        [JsonPropertyName("idscope")]
        public IdScope IdScope { get; set; }
    }

    /// <summary>
    /// Scope containing "idscope", restricting access based on internalIds for some other resource.
    /// </summary>
    public class WithIdScopeString : BaseScope
    {
        /// <summary>
        /// Restrict access based on ids for some other resource.
        /// </summary>
        [JsonPropertyName("idscope")]
        public IdScopeString IdScope { get; set; }
    }

    /// <summary>
    /// Scope with current user object.
    /// </summary>
    public class WithCurrentUserScope : BaseScope
    {
        /// <summary>
        /// Scope access to current user.
        /// </summary>
        [JsonPropertyName("currentuserscope")]
        public EmptyScope CurrentUserScope { get; set; }
    }

    /// <summary>
    /// Scope object with list of data set ids.
    /// </summary>
    public class WithDataSetsScope : BaseScope
    {
        /// <summary>
        /// Restrict access based on list of data set ids.
        /// </summary>
        public IdScope DatasetScope { get; set; }
    }

    /// <summary>
    /// Scope access based on list of internal ids or data set ids.
    /// </summary>
    public class WithIdAndDatasetScope : WithIdScope
    {
        /// <summary>
        /// Restrict access based on list of data set ids.
        /// </summary>
        public IdScope DatasetScope { get; set; }
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

        private static readonly JsonSerializerOptions _nestedOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

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

            BaseAcl result;

            if (_aclTypes.TryGetValue(aclName, out var type))
            {
                result = JsonSerializer.Deserialize(ref reader, type, _nestedOptions) as BaseAcl;
            }
            else
            {
                result = JsonSerializer.Deserialize<BaseAcl>(ref reader, _nestedOptions);
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException("Expected end of object after acl");
            }

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

            JsonSerializer.Serialize(writer, value, type, _nestedOptions);

            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// Acl for access to the groups resource. 
    /// </summary>
    public class GroupsAcl : BaseAcl<WithCurrentUserScope> { }
    
    /// <summary>
    /// Acl for access to the assets resource.
    /// </summary>
    public class AssetsAcl : BaseAcl<WithDataSetsScope> { }

    /// <summary>
    /// Acl for access to the events resource.
    /// </summary>
    public class EventsAcl : BaseAcl<WithDataSetsScope> { }

    /// <summary>
    /// Acl for access to the files resource.
    /// </summary>
    public class FilesAcl : BaseAcl<WithDataSetsScope> { }

    /// <summary>
    /// Acl for access to the users resource.
    /// </summary>
    public class UsersAcl : BaseAcl<WithCurrentUserScope> { }

    /// <summary>
    /// Acl for access to the projects resource.
    /// </summary>
    public class ProjectsAcl : BaseAcl<BaseScope> { }

    /// <summary>
    /// Acl for access to the security categories resource.
    /// </summary>
    public class SecurityCategoriesAcl : BaseAcl<WithIdScopeString> { }

    /// <summary>
    /// Acl for access to the raw resource.
    /// </summary>
    public class RawAcl : BaseAcl<WithRawTableScope> { }

    /// <summary>
    /// Scope for access to timeseries.
    /// </summary>
    public class TimeSeriesScope : WithIdScope
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
    /// Acl for access to the timeseries resource.
    /// </summary>
    public class TimeSeriesAcl : BaseAcl<TimeSeriesScope> { }

    /// <summary>
    /// Acl for access to the api keys resource.
    /// </summary>
    public class ApiKeysAcl : BaseAcl<WithIdScopeString> { }

    /// <summary>
    /// Acl for access to the 3d resource.
    /// </summary>
    public class ThreedAcl : BaseAcl<BaseScope> { }

    /// <summary>
    /// Acl for access to the sequences resource.
    /// </summary>
    public class SequencesAcl : BaseAcl<WithDataSetsScope> { }

    /// <summary>
    /// Acl for access to the labels resource.
    /// </summary>
    public class LabelsAcl : BaseAcl<WithDataSetsScope> { }

    /// <summary>
    /// Acl for access to the analytics resource.
    /// </summary>
    public class AnalyticsAcl : BaseAcl<BaseScope> { }

    /// <summary>
    /// Acl for access to the digital twin resource.
    /// </summary>
    public class DigitalTwinAcl : BaseAcl<BaseScope> { }

    /// <summary>
    /// Acl for access to the relationships resource.
    /// </summary>
    public class RelationshipsAcl : BaseAcl<WithDataSetsScope> { }

    /// <summary>
    /// Acl for access to the datasets resource.
    /// </summary>
    public class DatasetsAcl : BaseAcl<WithIdScope> { }

    /// <summary>
    /// Acl for access to the seismic resource.
    /// </summary>
    public class SeismicAcl : BaseAcl<BaseScope> { }

    /// <summary>
    /// Acl for access to the types resource.
    /// </summary>
    public class TypesAcl : BaseAcl<BaseScope> { }

    /// <summary>
    /// Acl for access to the model hosting resource.
    /// </summary>
    public class ModelHostingAcl : BaseAcl<BaseScope> { }

    /// <summary>
    /// Acl for access to the functions resource.
    /// </summary>
    public class FunctionsAcl : BaseAcl<BaseScope> { }

    /// <summary>
    /// Acl for access to the extraction pipelines resource.
    /// </summary>
    public class ExtPipesAcl : BaseAcl<WithIdAndDatasetScope> { }

    /// <summary>
    /// Scope for access to the extraction pipeline runs resource.
    /// </summary>
    public class ExtPipeRunsScope : WithDataSetsScope
    {
        /// <summary>
        /// Scope by list of extraction pipeline internal ids.
        /// </summary>
        [JsonPropertyName("extractionpipelinescope")]
        public IdScope ExtractionPipelineScope { get; set; }
    }

    /// <summary>
    /// Acl for access to the extraction pipeline runs resource.
    /// </summary>
    public class ExtPipeRunsAcl : BaseAcl<ExtPipeRunsScope> { }
}
