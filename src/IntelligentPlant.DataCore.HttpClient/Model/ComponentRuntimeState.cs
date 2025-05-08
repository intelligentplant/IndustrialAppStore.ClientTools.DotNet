namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the current runtime state of a Data Core component.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum ComponentRuntimeState {

        /// <summary>
        /// The current state of the component is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The component is disabled.
        /// </summary>
        Disabled = 1,

        /// <summary>
        /// The component is stopped.
        /// </summary>
        Stopped = 2,

        /// <summary>
        /// The component is initialising.
        /// </summary>
        Initializing = 3,

        /// <summary>
        /// The component is running.
        /// </summary>
        Running = 4,

        /// <summary>
        /// The component is stopping.
        /// </summary>
        Stopping = 5

    }
}
