﻿// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The Sequence column DTO.
    /// </summary>
    public class SequenceColumnWriteDto
    {
        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; }

        /// The externalId of the column. Must be unique within the project.
        public string ExternalId { get; set; }

        /// <summary>
        /// The description of the column.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The valueType of the column. Enum STRING, DOUBLE, LONG
        /// </summary>
        public SequenceValueType ValueType { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        public IDictionary<string, string> MetaData { get; set; }
    }
}