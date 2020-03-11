// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// File search name query class.
    /// </summary>
    public class NameSearch
    {
        /// <summary>
        /// Prefix and fuzzy search on name.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// File search query class.
    /// </summary>
    public class FileSearch : SearchQuery<FileFilter, NameSearch>
    {
        /// <summary>
        /// Create a new empty File search with pre-initialized emtpy Filter and Search.
        /// </summary>
        /// <returns>New instance of the FileSearch.</returns>
        public static FileSearch Empty ()
        {
            return new FileSearch {
                Filter=new FileFilter(),
                Search=new NameSearch()
            };
        }
    }
}

