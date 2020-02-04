// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The sequence column info DTO.
    /// </summary>
    public class SequenceColumnInfoDto
    {
        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; }

        /// The external id of the column. Must be unique within the project.
        public string ExternalId { get; set; }

        /// <summary>
        /// The value type of the column. Enum STRING, DOUBLE, LONG
        /// </summary>
        public MultiValueType ValueType { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SequenceColumnInfoDto>(this);
    }
}
