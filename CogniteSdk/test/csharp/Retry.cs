// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading.Tasks;

namespace Test.CSharp.Integration
{
    /// <summary>
    /// Retries an arbitrary async operation with exponential backoff and jitter.
    /// Intended for integration tests that hit eventually consistent or rate limited endpoints.
    /// </summary>
    public static class Retry
    {
        private const int DefaultMaxAttempts = 5;
        private static readonly TimeSpan BaseDelay = TimeSpan.FromMilliseconds(500);
        private static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Runs <paramref name="action"/> until it completes without throwing, or until
        /// <paramref name="maxAttempts"/> attempts have been made. The last exception is rethrown.
        /// </summary>
        /// <param name="action">Operation to run.</param>
        /// <param name="shouldRetry">Optional predicate deciding whether an exception is retryable. Defaults to retrying on any exception.</param>
        /// <param name="maxAttempts">Maximum number of attempts, including the first one.</param>
        public static async Task<T> RunAsync<T>(
            Func<Task<T>> action,
            Func<Exception, bool> shouldRetry = null,
            int maxAttempts = DefaultMaxAttempts)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (maxAttempts < 1) throw new ArgumentOutOfRangeException(nameof(maxAttempts));

            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    return await action().ConfigureAwait(false);
                }
                catch (Exception ex) when (attempt < maxAttempts && (shouldRetry?.Invoke(ex) ?? true))
                {
                    await Task.Delay(BackoffDelay(attempt)).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Non-generic overload of <see cref="RunAsync{T}"/> for operations that return no value.
        /// </summary>
        public static Task RunAsync(
            Func<Task> action,
            Func<Exception, bool> shouldRetry = null,
            int maxAttempts = DefaultMaxAttempts)
        {
            if (action is null) throw new ArgumentNullException(nameof(action));

            return RunAsync(async () =>
            {
                await action().ConfigureAwait(false);
                return true;
            }, shouldRetry, maxAttempts);
        }

        /// <summary>
        /// Exponential backoff with "equal jitter": half of the exponential delay is fixed,
        /// the other half is randomised, so concurrent retries spread out instead of stampeding.
        /// With the defaults this yields roughly 0.25-0.5s, 0.5-1s, 1-2s, 2-4s between attempts.
        /// </summary>
        private static TimeSpan BackoffDelay(int attempt)
        {
            var exponential = Math.Min(MaxDelay.TotalMilliseconds, BaseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
            var half = exponential / 2;
            return TimeSpan.FromMilliseconds(half + Random.Shared.NextDouble() * half);
        }
    }
}
