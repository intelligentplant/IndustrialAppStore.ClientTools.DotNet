namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Describes the basis on which data is retrieved when recalculating the snapshot value of a 
    /// script tag.
    /// </summary>
    public enum ScriptTagInputDataType {

        /// <summary>
        /// The recalculation is performed using the current values of referenced tags. For script 
        /// tags that are recalculated on a schedule, a values-at-times query is performed to get 
        /// the values of the input tags at the trigger time.
        /// </summary>
        CurrentValue = 0,

        /// <summary>
        /// The recalculation is performed using one or more aggregated data functions (e.g. 
        /// <see cref="DataFunctions.Average"/>, <see cref="DataFunctions.Minimum"/>, 
        /// <see cref="DataFunctions.Maximum"/>). The script tag will track the UTC sample time of 
        /// its most-recently-calculated value, and will recalculate from that time forwards until the 
        /// trigger timestamp (either the current UTC time, or the UTC time associated with a scheduled 
        /// trigger). The data functions and sample interval configured on the tag will determine which 
        /// aggregation types are requested, and how many times recalculation will be performed (e.g. if
        /// the sample interval is <c>5m</c>, and recalculation last happened 1 hour ago, data will be 
        /// requested from <c>*-1h</c> to <c>*</c>, at a <c>1h</c> sample interval, and the script tag 
        /// value will be recalculated and emitted at each sample time in that time range.
        /// </summary>
        AggregatedValues = 1,

        /// <summary>
        /// Like <see cref="AggregatedValues"/>, the recalculation is performed using aggregated 
        /// data. However, with <see cref="RollingAggregatedValues"/>, the aggregation is performed over 
        /// a time period that is larger than the interval between samples. In this mode, the interval 
        /// between samples is controlled using the block size configured on the script tag. For example,
        /// if the sample interval is <c>4h</c>, and the block size is <c>15m</c>, and recalculation last 
        /// happened 1 hour ago, aggregated data will be requested for each 15 minute block in the last 
        /// hour, with each block in turn requesting data calculated for the 4 hours leading up to the 
        /// block's timestamp.
        /// </summary>
        RollingAggregatedValues = 2,

        /// <summary>
        /// The recalculation is performed using raw historical data. The script tag will track the UTC 
        /// sample time of its most-recently-calculated value, and will recalculate from that time 
        /// forwards until the trigger timestamp (either the current UTC time, or the UTC time 
        /// associated with a scheduled trigger). Since historians typically place an absolute limit on 
        /// the maximum number of raw samples that can be returned per tag per query, you should ensure 
        /// that the input data block size used by the script tag allows the script host to receive all 
        /// the required raw values for its inputs. For example, if a historian returns an absolute 
        /// limit of 100 samples for raw queries, and a referenced tag is resampled every second, the 
        /// block size for the script tag should be no more than 100 seconds, to ensure that the script 
        /// tag is able to recalculate at every recorded sample time.
        /// </summary>
        RawValues = 3,

        /// <summary>
        /// The script tag does not require any pre-fetched data to perform its calculation. The 
        /// script tag can still request data from referenced tags on an ad hoc basis inside the 
        /// script.
        /// </summary>
        None = 4

    }
}
