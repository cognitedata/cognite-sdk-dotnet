// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Target units for a records filter, sync or aggregate request.
    ///
    /// Either set <see cref="UnitSystemName"/> to convert all convertible properties to a unit system,
    /// or set <see cref="Properties"/> to convert individual properties. Exactly one of the two must be set.
    /// </summary>
    public class StreamRecordTargetUnits
    {
        /// <summary>
        /// Name of the unit system. All properties that can be converted will be converted to this unit system.
        /// </summary>
        public string UnitSystemName { get; set; }
        /// <summary>
        /// List of properties and the units to convert them to.
        /// </summary>
        public IEnumerable<StreamRecordPropertyTargetUnit> Properties { get; set; }

        /// <summary>
        /// Convert all convertible properties to the given unit system.
        /// </summary>
        /// <param name="unitSystemName">Name of the unit system, for example "SI".</param>
        /// <returns>Target units specification.</returns>
        public static StreamRecordTargetUnits ForUnitSystem(string unitSystemName)
        {
            return new StreamRecordTargetUnits { UnitSystemName = unitSystemName };
        }

        /// <summary>
        /// Convert the given properties individually.
        /// </summary>
        /// <param name="properties">Properties and the units to convert them to.</param>
        /// <returns>Target units specification.</returns>
        public static StreamRecordTargetUnits ForProperties(IEnumerable<StreamRecordPropertyTargetUnit> properties)
        {
            return new StreamRecordTargetUnits { Properties = properties };
        }
    }

    /// <summary>
    /// A property and the unit or unit system to convert it to.
    /// </summary>
    public class StreamRecordPropertyTargetUnit
    {
        /// <summary>
        /// Property to convert. Format: [space, container, property], or
        /// [space, "viewExternalId/version", property] to address the property through a record view
        /// (see <see cref="DataModels.SourceIdentifier.PropertyReference(string)"/>).
        /// Top level properties are not supported.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// Target unit or unit system to convert the property to.
        /// </summary>
        public StreamRecordTargetUnit Unit { get; set; }
    }

    /// <summary>
    /// Target unit or unit system for a single property. Exactly one of
    /// <see cref="UnitSystemName"/> and <see cref="ExternalId"/> must be set.
    /// </summary>
    public class StreamRecordTargetUnit
    {
        /// <summary>
        /// Name of the unit system to convert the property to.
        /// </summary>
        public string UnitSystemName { get; set; }
        /// <summary>
        /// External ID of the unit to convert the property to, for example "temperature:k".
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Convert to the given unit system.
        /// </summary>
        /// <param name="unitSystemName">Name of the unit system, for example "SI".</param>
        /// <returns>Target unit specification.</returns>
        public static StreamRecordTargetUnit FromUnitSystem(string unitSystemName)
        {
            return new StreamRecordTargetUnit { UnitSystemName = unitSystemName };
        }

        /// <summary>
        /// Convert to the unit with the given external ID.
        /// </summary>
        /// <param name="externalId">External ID of the unit, for example "temperature:k".</param>
        /// <returns>Target unit specification.</returns>
        public static StreamRecordTargetUnit FromExternalId(string externalId)
        {
            return new StreamRecordTargetUnit { ExternalId = externalId };
        }
    }
}
