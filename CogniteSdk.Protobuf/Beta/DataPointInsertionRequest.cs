// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: data_point_insertion_request.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Com.Cognite.V1.Timeseries.Proto.Beta {

  /// <summary>Holder for reflection information generated from data_point_insertion_request.proto</summary>
  public static partial class DataPointInsertionRequestReflection {

    #region Descriptor
    /// <summary>File descriptor for data_point_insertion_request.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static DataPointInsertionRequestReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CiJkYXRhX3BvaW50X2luc2VydGlvbl9yZXF1ZXN0LnByb3RvEiRjb20uY29n",
            "bml0ZS52MS50aW1lc2VyaWVzLnByb3RvLmJldGEaEWRhdGFfcG9pbnRzLnBy",
            "b3RvIokCChZEYXRhUG9pbnRJbnNlcnRpb25JdGVtEgwKAmlkGAEgASgDSAAS",
            "FAoKZXh0ZXJuYWxJZBgCIAEoCUgAElQKEW51bWVyaWNEYXRhcG9pbnRzGAMg",
            "ASgLMjcuY29tLmNvZ25pdGUudjEudGltZXNlcmllcy5wcm90by5iZXRhLk51",
            "bWVyaWNEYXRhcG9pbnRzSAESUgoQc3RyaW5nRGF0YXBvaW50cxgEIAEoCzI2",
            "LmNvbS5jb2duaXRlLnYxLnRpbWVzZXJpZXMucHJvdG8uYmV0YS5TdHJpbmdE",
            "YXRhcG9pbnRzSAFCEAoOaWRPckV4dGVybmFsSWRCDwoNZGF0YXBvaW50VHlw",
            "ZSJoChlEYXRhUG9pbnRJbnNlcnRpb25SZXF1ZXN0EksKBWl0ZW1zGAEgAygL",
            "MjwuY29tLmNvZ25pdGUudjEudGltZXNlcmllcy5wcm90by5iZXRhLkRhdGFQ",
            "b2ludEluc2VydGlvbkl0ZW1CAlABYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointsReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionItem), global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionItem.Parser, new[]{ "Id", "ExternalId", "NumericDatapoints", "StringDatapoints" }, new[]{ "IdOrExternalId", "DatapointType" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionRequest), global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionRequest.Parser, new[]{ "Items" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class DataPointInsertionItem : pb::IMessage<DataPointInsertionItem>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<DataPointInsertionItem> _parser = new pb::MessageParser<DataPointInsertionItem>(() => new DataPointInsertionItem());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<DataPointInsertionItem> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionRequestReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public DataPointInsertionItem() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public DataPointInsertionItem(DataPointInsertionItem other) : this() {
      switch (other.IdOrExternalIdCase) {
        case IdOrExternalIdOneofCase.Id:
          Id = other.Id;
          break;
        case IdOrExternalIdOneofCase.ExternalId:
          ExternalId = other.ExternalId;
          break;
      }

      switch (other.DatapointTypeCase) {
        case DatapointTypeOneofCase.NumericDatapoints:
          NumericDatapoints = other.NumericDatapoints.Clone();
          break;
        case DatapointTypeOneofCase.StringDatapoints:
          StringDatapoints = other.StringDatapoints.Clone();
          break;
      }

      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public DataPointInsertionItem Clone() {
      return new DataPointInsertionItem(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public long Id {
      get { return HasId ? (long) idOrExternalId_ : 0L; }
      set {
        idOrExternalId_ = value;
        idOrExternalIdCase_ = IdOrExternalIdOneofCase.Id;
      }
    }
    /// <summary>Gets whether the "id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasId {
      get { return idOrExternalIdCase_ == IdOrExternalIdOneofCase.Id; }
    }
    /// <summary> Clears the value of the oneof if it's currently set to "id" </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearId() {
      if (HasId) {
        ClearIdOrExternalId();
      }
    }

    /// <summary>Field number for the "externalId" field.</summary>
    public const int ExternalIdFieldNumber = 2;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ExternalId {
      get { return HasExternalId ? (string) idOrExternalId_ : ""; }
      set {
        idOrExternalId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        idOrExternalIdCase_ = IdOrExternalIdOneofCase.ExternalId;
      }
    }
    /// <summary>Gets whether the "externalId" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasExternalId {
      get { return idOrExternalIdCase_ == IdOrExternalIdOneofCase.ExternalId; }
    }
    /// <summary> Clears the value of the oneof if it's currently set to "externalId" </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearExternalId() {
      if (HasExternalId) {
        ClearIdOrExternalId();
      }
    }

    /// <summary>Field number for the "numericDatapoints" field.</summary>
    public const int NumericDatapointsFieldNumber = 3;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Com.Cognite.V1.Timeseries.Proto.Beta.NumericDatapoints NumericDatapoints {
      get { return datapointTypeCase_ == DatapointTypeOneofCase.NumericDatapoints ? (global::Com.Cognite.V1.Timeseries.Proto.Beta.NumericDatapoints) datapointType_ : null; }
      set {
        datapointType_ = value;
        datapointTypeCase_ = value == null ? DatapointTypeOneofCase.None : DatapointTypeOneofCase.NumericDatapoints;
      }
    }

    /// <summary>Field number for the "stringDatapoints" field.</summary>
    public const int StringDatapointsFieldNumber = 4;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Com.Cognite.V1.Timeseries.Proto.Beta.StringDatapoints StringDatapoints {
      get { return datapointTypeCase_ == DatapointTypeOneofCase.StringDatapoints ? (global::Com.Cognite.V1.Timeseries.Proto.Beta.StringDatapoints) datapointType_ : null; }
      set {
        datapointType_ = value;
        datapointTypeCase_ = value == null ? DatapointTypeOneofCase.None : DatapointTypeOneofCase.StringDatapoints;
      }
    }

    private object idOrExternalId_;
    /// <summary>Enum of possible cases for the "idOrExternalId" oneof.</summary>
    public enum IdOrExternalIdOneofCase {
      None = 0,
      Id = 1,
      ExternalId = 2,
    }
    private IdOrExternalIdOneofCase idOrExternalIdCase_ = IdOrExternalIdOneofCase.None;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public IdOrExternalIdOneofCase IdOrExternalIdCase {
      get { return idOrExternalIdCase_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearIdOrExternalId() {
      idOrExternalIdCase_ = IdOrExternalIdOneofCase.None;
      idOrExternalId_ = null;
    }

    private object datapointType_;
    /// <summary>Enum of possible cases for the "datapointType" oneof.</summary>
    public enum DatapointTypeOneofCase {
      None = 0,
      NumericDatapoints = 3,
      StringDatapoints = 4,
    }
    private DatapointTypeOneofCase datapointTypeCase_ = DatapointTypeOneofCase.None;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public DatapointTypeOneofCase DatapointTypeCase {
      get { return datapointTypeCase_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearDatapointType() {
      datapointTypeCase_ = DatapointTypeOneofCase.None;
      datapointType_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as DataPointInsertionItem);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(DataPointInsertionItem other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (ExternalId != other.ExternalId) return false;
      if (!object.Equals(NumericDatapoints, other.NumericDatapoints)) return false;
      if (!object.Equals(StringDatapoints, other.StringDatapoints)) return false;
      if (IdOrExternalIdCase != other.IdOrExternalIdCase) return false;
      if (DatapointTypeCase != other.DatapointTypeCase) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (HasId) hash ^= Id.GetHashCode();
      if (HasExternalId) hash ^= ExternalId.GetHashCode();
      if (datapointTypeCase_ == DatapointTypeOneofCase.NumericDatapoints) hash ^= NumericDatapoints.GetHashCode();
      if (datapointTypeCase_ == DatapointTypeOneofCase.StringDatapoints) hash ^= StringDatapoints.GetHashCode();
      hash ^= (int) idOrExternalIdCase_;
      hash ^= (int) datapointTypeCase_;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (HasId) {
        output.WriteRawTag(8);
        output.WriteInt64(Id);
      }
      if (HasExternalId) {
        output.WriteRawTag(18);
        output.WriteString(ExternalId);
      }
      if (datapointTypeCase_ == DatapointTypeOneofCase.NumericDatapoints) {
        output.WriteRawTag(26);
        output.WriteMessage(NumericDatapoints);
      }
      if (datapointTypeCase_ == DatapointTypeOneofCase.StringDatapoints) {
        output.WriteRawTag(34);
        output.WriteMessage(StringDatapoints);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (HasId) {
        output.WriteRawTag(8);
        output.WriteInt64(Id);
      }
      if (HasExternalId) {
        output.WriteRawTag(18);
        output.WriteString(ExternalId);
      }
      if (datapointTypeCase_ == DatapointTypeOneofCase.NumericDatapoints) {
        output.WriteRawTag(26);
        output.WriteMessage(NumericDatapoints);
      }
      if (datapointTypeCase_ == DatapointTypeOneofCase.StringDatapoints) {
        output.WriteRawTag(34);
        output.WriteMessage(StringDatapoints);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (HasId) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(Id);
      }
      if (HasExternalId) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ExternalId);
      }
      if (datapointTypeCase_ == DatapointTypeOneofCase.NumericDatapoints) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(NumericDatapoints);
      }
      if (datapointTypeCase_ == DatapointTypeOneofCase.StringDatapoints) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(StringDatapoints);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(DataPointInsertionItem other) {
      if (other == null) {
        return;
      }
      switch (other.IdOrExternalIdCase) {
        case IdOrExternalIdOneofCase.Id:
          Id = other.Id;
          break;
        case IdOrExternalIdOneofCase.ExternalId:
          ExternalId = other.ExternalId;
          break;
      }

      switch (other.DatapointTypeCase) {
        case DatapointTypeOneofCase.NumericDatapoints:
          if (NumericDatapoints == null) {
            NumericDatapoints = new global::Com.Cognite.V1.Timeseries.Proto.Beta.NumericDatapoints();
          }
          NumericDatapoints.MergeFrom(other.NumericDatapoints);
          break;
        case DatapointTypeOneofCase.StringDatapoints:
          if (StringDatapoints == null) {
            StringDatapoints = new global::Com.Cognite.V1.Timeseries.Proto.Beta.StringDatapoints();
          }
          StringDatapoints.MergeFrom(other.StringDatapoints);
          break;
      }

      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Id = input.ReadInt64();
            break;
          }
          case 18: {
            ExternalId = input.ReadString();
            break;
          }
          case 26: {
            global::Com.Cognite.V1.Timeseries.Proto.Beta.NumericDatapoints subBuilder = new global::Com.Cognite.V1.Timeseries.Proto.Beta.NumericDatapoints();
            if (datapointTypeCase_ == DatapointTypeOneofCase.NumericDatapoints) {
              subBuilder.MergeFrom(NumericDatapoints);
            }
            input.ReadMessage(subBuilder);
            NumericDatapoints = subBuilder;
            break;
          }
          case 34: {
            global::Com.Cognite.V1.Timeseries.Proto.Beta.StringDatapoints subBuilder = new global::Com.Cognite.V1.Timeseries.Proto.Beta.StringDatapoints();
            if (datapointTypeCase_ == DatapointTypeOneofCase.StringDatapoints) {
              subBuilder.MergeFrom(StringDatapoints);
            }
            input.ReadMessage(subBuilder);
            StringDatapoints = subBuilder;
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            Id = input.ReadInt64();
            break;
          }
          case 18: {
            ExternalId = input.ReadString();
            break;
          }
          case 26: {
            global::Com.Cognite.V1.Timeseries.Proto.Beta.NumericDatapoints subBuilder = new global::Com.Cognite.V1.Timeseries.Proto.Beta.NumericDatapoints();
            if (datapointTypeCase_ == DatapointTypeOneofCase.NumericDatapoints) {
              subBuilder.MergeFrom(NumericDatapoints);
            }
            input.ReadMessage(subBuilder);
            NumericDatapoints = subBuilder;
            break;
          }
          case 34: {
            global::Com.Cognite.V1.Timeseries.Proto.Beta.StringDatapoints subBuilder = new global::Com.Cognite.V1.Timeseries.Proto.Beta.StringDatapoints();
            if (datapointTypeCase_ == DatapointTypeOneofCase.StringDatapoints) {
              subBuilder.MergeFrom(StringDatapoints);
            }
            input.ReadMessage(subBuilder);
            StringDatapoints = subBuilder;
            break;
          }
        }
      }
    }
    #endif

  }

  [global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
  public sealed partial class DataPointInsertionRequest : pb::IMessage<DataPointInsertionRequest>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<DataPointInsertionRequest> _parser = new pb::MessageParser<DataPointInsertionRequest>(() => new DataPointInsertionRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<DataPointInsertionRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionRequestReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public DataPointInsertionRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public DataPointInsertionRequest(DataPointInsertionRequest other) : this() {
      items_ = other.items_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public DataPointInsertionRequest Clone() {
      return new DataPointInsertionRequest(this);
    }

    /// <summary>Field number for the "items" field.</summary>
    public const int ItemsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionItem> _repeated_items_codec
        = pb::FieldCodec.ForMessage(10, global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionItem.Parser);
    private readonly pbc::RepeatedField<global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionItem> items_ = new pbc::RepeatedField<global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionItem>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::Com.Cognite.V1.Timeseries.Proto.Beta.DataPointInsertionItem> Items {
      get { return items_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as DataPointInsertionRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(DataPointInsertionRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!items_.Equals(other.items_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= items_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      items_.WriteTo(output, _repeated_items_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      items_.WriteTo(ref output, _repeated_items_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      size += items_.CalculateSize(_repeated_items_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(DataPointInsertionRequest other) {
      if (other == null) {
        return;
      }
      items_.Add(other.items_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            items_.AddEntriesFrom(input, _repeated_items_codec);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            items_.AddEntriesFrom(ref input, _repeated_items_codec);
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
