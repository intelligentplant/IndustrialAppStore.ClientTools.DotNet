using System;
using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes a Data Core real-time subscription.
    /// </summary>
    public class RealTimeDataSubscriptionInfo {

        /// <summary>
        /// Gets the UTC creation time for the subscription.
        /// </summary>
        public DateTime UtcCreationTime { get; }

        /// <summary>
        /// Gets the UTC time that the subscription last received a value at from a tag that it is observing.
        /// </summary>
        public DateTime UtcLastValueReceived { get; }

        /// <summary>
        /// Gets the UTC time that the subscription last emitted values at.
        /// </summary>
        public DateTime UtcLastEmit { get; }

        /// <summary>
        /// Gets the emit interval for the subscription.
        /// </summary>
        public TimeSpan EmitInterval { get; }

        /// <summary>
        /// Gets the tag subscriptions.  This is a map from data source name to tag name, and then 
        /// from tag name to the number of times the tag has been added to the subscription.
        /// </summary>
        public IDictionary<string, IDictionary<string, int>> TagSubscriptions { get; }


        /// <summary>
        /// Creates a new <see cref="RealTimeDataSubscriptionInfo"/> object.
        /// </summary>
        /// <param name="utcCreationTime">The UTC creation time for the subscription.</param>
        /// <param name="utcLastValueReceived">The UTC time that the subscription last received a value at from a tag that it is observing.</param>
        /// <param name="utcLastEmit">The UTC time that the subscription last emitted values at.</param>
        /// <param name="emitInterval">The emit interval for the subscription (i.e. how frequently it will push new values).</param>
        /// <param name="tagSubscriptions">The tag subscriptions.</param>
        public RealTimeDataSubscriptionInfo(DateTime utcCreationTime, DateTime utcLastValueReceived, DateTime utcLastEmit, TimeSpan emitInterval, IDictionary<string, IDictionary<string, int>> tagSubscriptions) {
            UtcCreationTime = utcCreationTime;
            UtcLastValueReceived = utcLastValueReceived;
            UtcLastEmit = utcLastEmit;
            EmitInterval = emitInterval;
            TagSubscriptions = tagSubscriptions ?? new Dictionary<string, IDictionary<string, int>>();
        }

    }
}
