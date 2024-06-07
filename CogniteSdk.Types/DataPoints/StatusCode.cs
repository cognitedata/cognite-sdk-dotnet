// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    // NOTE: This entire section is largely taken from the time series backend,
    // it more or less replicates the behavior of the API when it comes to parsing and handling status codes.
    // This is necessary for sanitation.

    /// <summary>
    /// A data point status code with code and/or corresponding symbol.
    /// The default status code is Good (0)
    /// </summary>
    class RawStatusCode
    {
        /// <summary>
        /// The numeric status code of the data point.
        /// </summary>
        public ulong? Code { get; set; }
        /// <summary>
        /// The status name of the data point.
        /// </summary>
        public string Symbol { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Json converter for status codes.
    /// </summary>
    public class StatusCodeConverter : JsonConverter<StatusCode>
    {
        /// <inheritdoc />
        public override StatusCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return StatusCode.FromCategory(StatusCodeCategory.Good);
            }

            var raw = JsonSerializer.Deserialize<RawStatusCode>(ref reader, options);

            if (raw.Code.HasValue)
            {
                return new StatusCode(raw.Code.Value);
            }
            else if (raw.Symbol != null)
            {
                return StatusCode.Parse(raw.Symbol);
            }
            else
            {
                return StatusCode.FromCategory(StatusCodeCategory.Good);
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, StatusCode value, JsonSerializerOptions options)
        {
            var raw = new RawStatusCode
            {
                Code = value.Code
            };
            JsonSerializer.Serialize(writer, raw, options);
        }
    }

    /// <summary>
    /// Helper methods for dealing with status codes.
    /// </summary>
    internal static class StatusCodeHelpers
    {
        private static HashSet<string> _supportedFlags = new HashSet<string> {
            "Low", "High", "Constant",
            "StructureChanged", "SemanticsChanged", "Overflow",
            "MultipleValues", "ExtraData", "Partial", "Interpolated", "Calculated",
        };

        /// <summary>
        /// Attempt to parse a status code symbol into a numerical status code.
        /// </summary>
        /// <param name="symbol">Status code symbol</param>
        /// <param name="value">Parsed value, or null if parsing failed</param>
        /// <returns>A string containing the error that caused parsing to fail</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string TryParseStatusCodeSymbol(string symbol, out StatusCode? value)
        {
            if (symbol == null) throw new ArgumentNullException(nameof(symbol));

            value = null;

            var parts = symbol.Split(' ', ',');
            if (!Enum.TryParse(parts[0].Replace("Good_", "Good")
                .Replace("Bad_", "Bad")
                .Replace("Uncertain_", "Uncertain"), out StatusCodeCategory category))
            {
                return "Unknown severity or sub code";
            }

            foreach (var p in parts.Skip(1).Where((p) => !string.IsNullOrWhiteSpace(p)))
            {
                if (!_supportedFlags.Contains(p))
                {
                    return $"Unsupported flag {p}";
                }
            }

            var code = (ulong)category;

            if (parts.Contains("StructureChanged"))
            {
                code |= 1 << 15;
            }
            if (parts.Contains("SemanticsChanged"))
            {
                code |= 1 << 14;
            }

            var limitRes = TryParseLimit(parts, out var limitValue);
            if (limitRes != null) return limitRes;

            var historianRes = TryParseHistorianBits(parts, out var historianValue);
            if (historianRes != null) return historianRes;

            code |= limitValue!.Value | historianValue!.Value;

            value = new StatusCode(code);

            return null;
        }

        private static string TryParseLimit(string[] parts, out ulong? value)
        {
            value = 0;
            int limitArguments = 0;
            if (parts.Contains("Low"))
            {
                value = 0x100L;
                limitArguments++;
            }
            if (parts.Contains("High"))
            {
                value = 0x200L;
                limitArguments++;
            }
            if (parts.Contains("Constant"))
            {
                value = 0x300L;
                limitArguments++;
            }

            if (limitArguments > 1)
            {
                return "More than one status code limit";
            }

            if (value > 0) value |= 1 << 10;

            return null;
        }

        private static string TryParseHistorianBits(string[] parts, out ulong? value)
        {
            value = 0;
            if (parts.Contains("Partial")) value |= 0b100;
            if (parts.Contains("MultipleValues")) value |= 0b1_0000;
            if (parts.Contains("ExtraData")) value |= 0b1000;
            if (parts.Contains("Overflow")) value |= 1 << 7;

            var isInterpolated = parts.Contains("Interpolated");
            var isCalculated = parts.Contains("Calculated");
            if (isInterpolated && isCalculated)
            {
                return "Calculated and Interpolated flags are mutually exclusive";
            }

            if (isCalculated) value |= 1;
            if (isInterpolated) value |= 0b10;

            if (value > 0) value |= 1 << 10;

            return null;
        }

        internal const ulong CATEGORY_MASK = 0b1111_1111_1111_1111_0000_0000_0000_0000L;
        internal const ulong INFO_BITS_MASK = 0b0000_0000_0000_0000_0000_0011_1111_1111L;
    }

    /// <summary>
    /// Exception thrown by trying to create an invalid status code.
    /// </summary>
    public class InvalidStatusCodeException : Exception
    {
        /// <inheritdocs />
        public InvalidStatusCodeException(string message) : base(message)
        {
        }

        /// <inheritdocs />
        public InvalidStatusCodeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdocs />
        public InvalidStatusCodeException()
        {
        }
    }

    /// <summary>
    /// Representation of a status code, deconstructed.
    /// </summary>
    public struct StatusCode
    {
        private ulong _code;

        /// <summary>
        /// Status code
        /// </summary>
        public readonly ulong Code => _code;

        /// <summary>
        /// Status code symbol.
        /// </summary>
        /// <returns></returns>
        public readonly string Symbol => ToString();

        internal StatusCode(ulong code)
        {
            _code = code;
        }

        /// <summary>
        /// Create a status code from a category.
        /// </summary>
        /// <param name="category">Category to create from</param>
        /// <returns>Status code</returns>
        public static StatusCode FromCategory(StatusCodeCategory category)
        {
            return new StatusCode((ulong)category);
        }


        /// <summary>
        /// Try to parse a status code from a string.
        /// </summary>
        /// <param name="symbol">Status code symbol to parse</param>
        /// <param name="result">Status code, if parsing succeeded, else null</param>
        /// <returns>Error if parsing failed</returns>
        public static string TryParse(string symbol, out StatusCode? result)
        {
            return StatusCodeHelpers.TryParseStatusCodeSymbol(symbol, out result);
        }

        /// <summary>
        /// Parse a status code from a string, throw an exception if it fails.
        /// </summary>
        /// <param name="symbol">Status code symbol to parse</param>
        /// <returns>Parsed status code</returns>
        /// <exception cref="InvalidStatusCodeException"></exception>
        public static StatusCode Parse(string symbol)
        {
            var res = TryParse(symbol, out var result);

            if (res != null) throw new InvalidStatusCodeException(res);

            return result!.Value;
        }

        /// <summary>
        /// Try create a status code from a long.
        /// </summary>
        /// <param name="code">Status code symbol to parse</param>
        /// <param name="result">Status code, if parsing succeeded, else null</param>
        /// <returns>Error if parsing failed</returns>
        public static string TryCreate(ulong code, out StatusCode? result)
        {
            if (code == 0)
            {
                result = new StatusCode(code);
                return null;
            }

            result = null;

            if ((code >> 30) == 0b11) return "Unsupported severity: 0b11";
            if ((code & (0b11 << 28)) != 0) return "Bits 28 and 29 are reserved";
            if (!Enum.IsDefined(typeof(StatusCodeCategory), code & StatusCodeHelpers.CATEGORY_MASK))
            {
                return "Unknown category";
            }

            if ((code & (0b111 << 1)) != 0) return "Bits 11, 12, and 13 are reserved";
            if ((code & (0b11 << 5)) != 0) return "Bits 5 and 6 are reserved";

            var infoBits = StatusCodeHelpers.INFO_BITS_MASK & code;
            if ((code & (1 << 10)) != 0)
            {
                if ((code & 0b11) == 0b11)
                {
                    return "Calculated and Interpolated flags are mutually exclusive";
                }
            }
            else if (infoBits != 0)
            {
                return "When info type is 00, all info bits must be 0";
            }

            result = new StatusCode(code);

            return null;
        }

        /// <summary>
        /// Parse a status code from a numerical code, throw an exception if it fails.
        /// </summary>
        /// <param name="code">Status code to parse</param>
        /// <returns>Parsed status code</returns>
        /// <exception cref="InvalidStatusCodeException"></exception>
        public static StatusCode Create(ulong code)
        {
            var res = TryCreate(code, out var result);

            if (res != null) throw new InvalidStatusCodeException(res);

            return result!.Value;
        }

        /// <inheritdoc />
        public static bool operator ==(StatusCode lhs, StatusCode rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <inheritdoc />
        public static bool operator !=(StatusCode lhs, StatusCode rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc />
        public override readonly bool Equals(object obj)
        {
            return obj is StatusCode code && code.Code == _code;
        }

        /// <inheritdoc />
        public override readonly int GetHashCode()
        {
            return _code.GetHashCode();
        }

        /// <inheritdoc />
        public override readonly string ToString()
        {
            if (_code == 0) return "Good";
            var builder = new StringBuilder();

            builder.Append(Category);
            if (StructureChanged) builder.Append(", StructureChanged");
            if (SemanticsChanged) builder.Append(", SemanticsChanged");
            if (Limit != Limit.None) builder.AppendFormat(", {0}", Limit);
            if (IsOverflow) builder.Append(", Overflow");
            if (IsMultiValue) builder.Append(", MultipleValues");
            if (HasExtraData) builder.Append(", ExtraData");
            if (IsPartial) builder.Append(", Partial");
            if (ValueType != ValueType.Raw) builder.AppendFormat(", {0}", ValueType);

            return builder.ToString();
        }

        private void SetBool(bool value, byte offset)
        {
            _code = _code & ~(1ul << offset) | ((value ? 1ul : 0ul) << offset);
        }

        private readonly bool GetBool(byte offset)
        {
            return (_code & (1ul << offset)) != 0;
        }

        /// <summary>
        /// Whether this status code is good.
        /// </summary>
        public readonly bool IsGood => Severity == Severity.Good;

        /// <summary>
        /// Whether this status code is bad.
        /// </summary>
        public readonly bool IsBad => Severity == Severity.Bad;

        /// <summary>
        /// Whether this status code is uncertain.
        /// </summary>
        public readonly bool IsUncertain => Severity == Severity.Uncertain;

        /// <summary>
        /// Type of status code.
        /// </summary>
        public Severity Severity
        {
            readonly get => (Severity)((_code >> 30) & 0b11);
            set => _code = _code & ~StatusCodeHelpers.CATEGORY_MASK | ((((ulong)value) & 0b11) << 30);
        }

        /// <summary>
        /// Structure changed flag.
        /// </summary>
        public bool StructureChanged
        {
            readonly get => GetBool(15);
            set => SetBool(value, 15);
        }
        /// <summary>
        /// Semantics changed flag.
        /// </summary>
        public bool SemanticsChanged
        {
            readonly get => GetBool(14);
            set => SetBool(value, 14);
        }

        /// <summary>
        /// Whether this is a data value info type.
        /// </summary>
        public bool IsDataValueInfoType
        {
            readonly get => GetBool(10);
            set => SetBool(value, 10);
        }

        /// <summary>
        /// Whether the value is bounded by some limit.
        /// </summary>
        public Limit Limit
        {
            readonly get => (Limit)((_code >> 8) & 0b11);
            set => _code = _code & ~(0b11ul << 8) | ((((ulong)value) & 0b11) << 8);
        }

        /// <summary>
        /// Status code category.
        /// </summary>
        public StatusCodeCategory Category
        {
            readonly get => (StatusCodeCategory)(_code & StatusCodeHelpers.CATEGORY_MASK);
            set => _code = _code & ~StatusCodeHelpers.CATEGORY_MASK | ((ulong)value) & StatusCodeHelpers.CATEGORY_MASK;
        }

        /// <summary>
        /// Whether the value is overflowed.
        /// </summary>
        public bool IsOverflow
        {
            readonly get => GetBool(7);
            set => SetBool(value, 7);
        }

        /// <summary>
        /// Multi value flag.
        /// </summary>
        public bool IsMultiValue
        {
            readonly get => GetBool(4);
            set => SetBool(value, 4);
        }
        /// <summary>
        /// Has extra data flag.
        /// </summary>
        public bool HasExtraData
        {
            readonly get => GetBool(3);
            set => SetBool(value, 3);
        }
        /// <summary>
        /// Is partial flag.
        /// </summary>
        public bool IsPartial
        {
            readonly get => GetBool(2);
            set => SetBool(value, 2);
        }

        /// <summary>
        /// Type of value origin.
        /// </summary>
        public ValueType ValueType
        {
            readonly get => (ValueType)(_code & 0b11);
            set => _code = _code & ~0b11ul | ((ulong)value) & 0b11ul;
        }

        /// <summary>
        /// Status code with the symbol "Good". This is the default in the time series API.
        /// </summary>
        public static StatusCode Good => FromCategory(StatusCodeCategory.Good);

        /// <summary>
        /// Status code with the symbol "Bad".
        /// </summary>
        public static StatusCode Bad => FromCategory(StatusCodeCategory.Bad);

        /// <summary>
        /// Status code with the symbol "Uncertain".
        /// </summary>
        public static StatusCode Uncertain => FromCategory(StatusCodeCategory.Uncertain);
    }

    /// <summary>
    /// Base status type: good, bad, or uncertain.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Status code is good.
        /// </summary>
        Good = 0,
        /// <summary>
        /// Status code is uncertain.
        /// </summary>
        Uncertain = 1,
        /// <summary>
        /// Status code is bad.
        /// </summary>
        Bad = 2
    }

    /// <summary>
    /// Enum for status code value source.
    /// </summary>
    public enum ValueType
    {
        /// <summary>
        /// Value is calculated.
        /// </summary>
        Calculated = 1,
        /// <summary>
        /// Value is interpolated.
        /// </summary>
        Interpolated = 2,
        /// <summary>
        /// Value is a raw value.
        /// </summary>
        Raw = 0,
    }

    /// <summary>
    /// Enum for status code limit types
    /// </summary>
    public enum Limit
    {
        /// <summary>
        /// Value is constant
        /// </summary>
        Constant = 0b11,
        /// <summary>
        /// Value is at high limit.
        /// </summary>
        High = 0b10,
        /// <summary>
        /// Value is at low limit.
        /// </summary>
        Low = 0b01,
        /// <summary>
        /// Value is not at a limit.
        /// </summary>
        None = 0b00
    }


    /// <summary>
    /// Enum of all status code categories.
    /// </summary>
    public enum StatusCodeCategory : ulong
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Good = 0x00000000,
        GoodCallAgain = 0x00A90000,
        GoodCascade = 0x04090000,
        GoodCascadeInitializationAcknowledged = 0x04010000,
        GoodCascadeInitializationRequest = 0x04020000,
        GoodCascadeNotInvited = 0x04030000,
        GoodCascadeNotSelected = 0x04040000,
        GoodClamped = 0x00300000,
        GoodCommunicationEvent = 0x00A70000,
        GoodCompletesAsynchronously = 0x002E0000,
        GoodDataIgnored = 0x00D90000,
        GoodDependentValueChanged = 0x00E00000,
        GoodEdited = 0x00DC0000,
        GoodEdited_DependentValueChanged = 0x01160000,
        GoodEdited_DominantValueChanged = 0x01170000,
        GoodEdited_DominantValueChanged_DependentValueChanged = 0x01180000,
        GoodEntryInserted = 0x00A20000,
        GoodEntryReplaced = 0x00A30000,
        GoodFaultStateActive = 0x04070000,
        GoodInitiateFaultState = 0x04080000,
        GoodLocalOverride = 0x00960000,
        GoodMoreData = 0x00A60000,
        GoodNoData = 0x00A50000,
        GoodNonCriticalTimeout = 0x00AA0000,
        GoodOverload = 0x002F0000,
        GoodPostActionFailed = 0x00DD0000,
        GoodResultsMayBeIncomplete = 0x00BA0000,
        GoodRetransmissionQueueNotSupported = 0x00DF0000,
        GoodShutdownEvent = 0x00A80000,
        GoodSubscriptionTransferred = 0x002D0000,
        Uncertain = 0x40000000,
        UncertainConfigurationError = 0x420F0000,
        UncertainDataSubNormal = 0x40A40000,
        UncertainDependentValueChanged = 0x40E20000,
        UncertainDominantValueChanged = 0x40DE0000,
        UncertainEngineeringUnitsExceeded = 0x40940000,
        UncertainInitialValue = 0x40920000,
        UncertainLastUsableValue = 0x40900000,
        UncertainNoCommunicationLastUsableValue = 0x408F0000,
        UncertainNotAllNodesAvailable = 0x40C00000,
        UncertainReferenceNotDeleted = 0x40BC0000,
        UncertainReferenceOutOfServer = 0x406C0000,
        UncertainSensorCalibration = 0x420A0000,
        UncertainSensorNotAccurate = 0x40930000,
        UncertainSimulatedValue = 0x42090000,
        UncertainSubNormal = 0x40950000,
        UncertainSubstituteValue = 0x40910000,
        UncertainTransducerInManual = 0x42080000,
        Bad = 0x80000000,
        BadAggregateConfigurationRejected = 0x80DA0000,
        BadAggregateInvalidInputs = 0x80D60000,
        BadAggregateListMismatch = 0x80D40000,
        BadAggregateNotSupported = 0x80D50000,
        BadAlreadyExists = 0x81150000,
        BadApplicationSignatureInvalid = 0x80580000,
        BadArgumentsMissing = 0x80760000,
        BadAttributeIdInvalid = 0x80350000,
        BadBoundNotFound = 0x80D70000,
        BadBoundNotSupported = 0x80D80000,
        BadBrowseDirectionInvalid = 0x804D0000,
        BadBrowseNameDuplicated = 0x80610000,
        BadBrowseNameInvalid = 0x80600000,
        BadCertificateChainIncomplete = 0x810D0000,
        BadCertificateHostNameInvalid = 0x80160000,
        BadCertificateInvalid = 0x80120000,
        BadCertificateIssuerRevocationUnknown = 0x801C0000,
        BadCertificateIssuerRevoked = 0x801E0000,
        BadCertificateIssuerTimeInvalid = 0x80150000,
        BadCertificateIssuerUseNotAllowed = 0x80190000,
        BadCertificatePolicyCheckFailed = 0x81140000,
        BadCertificateRevocationUnknown = 0x801B0000,
        BadCertificateRevoked = 0x801D0000,
        BadCertificateTimeInvalid = 0x80140000,
        BadCertificateUntrusted = 0x801A0000,
        BadCertificateUriInvalid = 0x80170000,
        BadCertificateUseNotAllowed = 0x80180000,
        BadCommunicationError = 0x80050000,
        BadConditionAlreadyDisabled = 0x80980000,
        BadConditionAlreadyEnabled = 0x80CC0000,
        BadConditionAlreadyShelved = 0x80D10000,
        BadConditionBranchAlreadyAcked = 0x80CF0000,
        BadConditionBranchAlreadyConfirmed = 0x80D00000,
        BadConditionDisabled = 0x80990000,
        BadConditionNotShelved = 0x80D20000,
        BadConfigurationError = 0x80890000,
        BadConnectionClosed = 0x80AE0000,
        BadConnectionRejected = 0x80AC0000,
        BadContentFilterInvalid = 0x80480000,
        BadContinuationPointInvalid = 0x804A0000,
        BadDataEncodingInvalid = 0x80380000,
        BadDataEncodingUnsupported = 0x80390000,
        BadDataLost = 0x809D0000,
        BadDataTypeIdUnknown = 0x80110000,
        BadDataUnavailable = 0x809E0000,
        BadDeadbandFilterInvalid = 0x808E0000,
        BadDecodingError = 0x80070000,
        BadDependentValueChanged = 0x80E30000,
        BadDeviceFailure = 0x808B0000,
        BadDialogNotActive = 0x80CD0000,
        BadDialogResponseInvalid = 0x80CE0000,
        BadDisconnect = 0x80AD0000,
        BadDiscoveryUrlMissing = 0x80510000,
        BadDominantValueChanged = 0x80E10000,
        BadDuplicateReferenceNotAllowed = 0x80660000,
        BadEdited_OutOfRange = 0x81190000,
        BadEdited_OutOfRange_DominantValueChanged = 0x811C0000,
        BadEdited_OutOfRange_DominantValueChanged_DependentValueChanged = 0x811E0000,
        BadEncodingError = 0x80060000,
        BadEncodingLimitsExceeded = 0x80080000,
        BadEndOfStream = 0x80B00000,
        BadEntryExists = 0x809F0000,
        BadEventFilterInvalid = 0x80470000,
        BadEventIdUnknown = 0x809A0000,
        BadEventNotAcknowledgeable = 0x80BB0000,
        BadExpectedStreamToBlock = 0x80B40000,
        BadFilterElementInvalid = 0x80C40000,
        BadFilterLiteralInvalid = 0x80C50000,
        BadFilterNotAllowed = 0x80450000,
        BadFilterOperandCountMismatch = 0x80C30000,
        BadFilterOperandInvalid = 0x80490000,
        BadFilterOperatorInvalid = 0x80C10000,
        BadFilterOperatorUnsupported = 0x80C20000,
        BadHistoryOperationInvalid = 0x80710000,
        BadHistoryOperationUnsupported = 0x80720000,
        BadIdentityChangeNotSupported = 0x80C60000,
        BadIdentityTokenInvalid = 0x80200000,
        BadIdentityTokenRejected = 0x80210000,
        BadIndexRangeInvalid = 0x80360000,
        BadIndexRangeNoData = 0x80370000,
        BadInitialValue_OutOfRange = 0x811A0000,
        BadInsufficientClientProfile = 0x807C0000,
        BadInternalError = 0x80020000,
        BadInvalidArgument = 0x80AB0000,
        BadInvalidSelfReference = 0x80670000,
        BadInvalidState = 0x80AF0000,
        BadInvalidTimestamp = 0x80230000,
        BadInvalidTimestampArgument = 0x80BD0000,
        BadLicenseExpired = 0x810E0000,
        BadLicenseLimitsExceeded = 0x810F0000,
        BadLicenseNotAvailable = 0x81100000,
        BadMaxAgeInvalid = 0x80700000,
        BadMaxConnectionsReached = 0x80B70000,
        BadMessageNotAvailable = 0x807B0000,
        BadMethodInvalid = 0x80750000,
        BadMonitoredItemFilterInvalid = 0x80430000,
        BadMonitoredItemFilterUnsupported = 0x80440000,
        BadMonitoredItemIdInvalid = 0x80420000,
        BadMonitoringModeInvalid = 0x80410000,
        BadNoCommunication = 0x80310000,
        BadNoContinuationPoints = 0x804B0000,
        BadNoData = 0x809B0000,
        BadNoDataAvailable = 0x80B10000,
        BadNoDeleteRights = 0x80690000,
        BadNoEntryExists = 0x80A00000,
        BadNoMatch = 0x806F0000,
        BadNoSubscription = 0x80790000,
        BadNoValidCertificates = 0x80590000,
        BadNodeAttributesInvalid = 0x80620000,
        BadNodeClassInvalid = 0x805F0000,
        BadNodeIdExists = 0x805E0000,
        BadNodeIdInvalid = 0x80330000,
        BadNodeIdRejected = 0x805D0000,
        BadNodeIdUnknown = 0x80340000,
        BadNodeNotInView = 0x804E0000,
        BadNonceInvalid = 0x80240000,
        BadNotConnected = 0x808A0000,
        BadNotExecutable = 0x81110000,
        BadNotFound = 0x803E0000,
        BadNotImplemented = 0x80400000,
        BadNotReadable = 0x803A0000,
        BadNotSupported = 0x803D0000,
        BadNotTypeDefinition = 0x80C80000,
        BadNotWritable = 0x803B0000,
        BadNothingToDo = 0x800F0000,
        BadNumericOverflow = 0x81120000,
        BadObjectDeleted = 0x803F0000,
        BadOperationAbandoned = 0x80B30000,
        BadOutOfMemory = 0x80030000,
        BadOutOfRange = 0x803C0000,
        BadOutOfRange_DominantValueChanged = 0x811B0000,
        BadOutOfRange_DominantValueChanged_DependentValueChanged = 0x811D0000,
        BadOutOfService = 0x808D0000,
        BadParentNodeIdInvalid = 0x805B0000,
        BadProtocolVersionUnsupported = 0x80BE0000,
        BadQueryTooComplex = 0x806E0000,
        BadReferenceLocalOnly = 0x80680000,
        BadReferenceNotAllowed = 0x805C0000,
        BadReferenceTypeIdInvalid = 0x804C0000,
        BadRefreshInProgress = 0x80970000,
        BadRequestCancelledByClient = 0x802C0000,
        BadRequestCancelledByRequest = 0x805A0000,
        BadRequestHeaderInvalid = 0x802A0000,
        BadRequestInterrupted = 0x80840000,
        BadRequestNotAllowed = 0x80E40000,
        BadRequestNotComplete = 0x81130000,
        BadRequestTimeout = 0x80850000,
        BadRequestTooLarge = 0x80B80000,
        BadRequestTypeInvalid = 0x80530000,
        BadResourceUnavailable = 0x80040000,
        BadResponseTooLarge = 0x80B90000,
        BadSecureChannelClosed = 0x80860000,
        BadSecureChannelIdInvalid = 0x80220000,
        BadSecureChannelTokenUnknown = 0x80870000,
        BadSecurityChecksFailed = 0x80130000,
        BadSecurityModeInsufficient = 0x80E60000,
        BadSecurityModeRejected = 0x80540000,
        BadSecurityPolicyRejected = 0x80550000,
        BadSempahoreFileMissing = 0x80520000,
        BadSensorFailure = 0x808C0000,
        BadSequenceNumberInvalid = 0x80880000,
        BadSequenceNumberUnknown = 0x807A0000,
        BadServerHalted = 0x800E0000,
        BadServerIndexInvalid = 0x806A0000,
        BadServerNameMissing = 0x80500000,
        BadServerNotConnected = 0x800D0000,
        BadServerUriInvalid = 0x804F0000,
        BadServiceUnsupported = 0x800B0000,
        BadSessionClosed = 0x80260000,
        BadSessionIdInvalid = 0x80250000,
        BadSessionNotActivated = 0x80270000,
        BadShelvingTimeOutOfRange = 0x80D30000,
        BadShutdown = 0x800C0000,
        BadSourceNodeIdInvalid = 0x80640000,
        BadStateNotActive = 0x80BF0000,
        BadStructureMissing = 0x80460000,
        BadSubscriptionIdInvalid = 0x80280000,
        BadSyntaxError = 0x80B60000,
        BadTargetNodeIdInvalid = 0x80650000,
        BadTcpEndpointUrlInvalid = 0x80830000,
        BadTcpInternalError = 0x80820000,
        BadTcpMessageTooLarge = 0x80800000,
        BadTcpMessageTypeInvalid = 0x807E0000,
        BadTcpNotEnoughResources = 0x80810000,
        BadTcpSecureChannelUnknown = 0x807F0000,
        BadTcpServerTooBusy = 0x807D0000,
        BadTicketInvalid = 0x81200000,
        BadTicketRequired = 0x811F0000,
        BadTimeout = 0x800A0000,
        BadTimestampNotSupported = 0x80A10000,
        BadTimestampsToReturnInvalid = 0x802B0000,
        BadTooManyArguments = 0x80E50000,
        BadTooManyMatches = 0x806D0000,
        BadTooManyMonitoredItems = 0x80DB0000,
        BadTooManyOperations = 0x80100000,
        BadTooManyPublishRequests = 0x80780000,
        BadTooManySessions = 0x80560000,
        BadTooManySubscriptions = 0x80770000,
        BadTypeDefinitionInvalid = 0x80630000,
        BadTypeMismatch = 0x80740000,
        BadUnexpectedError = 0x80010000,
        BadUnknownResponse = 0x80090000,
        BadUserAccessDenied = 0x801F0000,
        BadUserSignatureInvalid = 0x80570000,
        BadViewIdUnknown = 0x806B0000,
        BadViewParameterMismatch = 0x80CA0000,
        BadViewTimestampInvalid = 0x80C90000,
        BadViewVersionInvalid = 0x80CB0000,
        BadWaitingForInitialData = 0x80320000,
        BadWaitingForResponse = 0x80B20000,
        BadWouldBlock = 0x80B50000,
        BadWriteNotSupported = 0x80730000
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}