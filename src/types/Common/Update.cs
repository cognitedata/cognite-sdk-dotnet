// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    public abstract class Update<T> {}

    public class SetUpdate<T> : Update<T> {
        public T Set { get; set; }
    }

    public class SetNullUpdate<T> : Update<T> {
        public T SetNull { get; set; }
    }
}