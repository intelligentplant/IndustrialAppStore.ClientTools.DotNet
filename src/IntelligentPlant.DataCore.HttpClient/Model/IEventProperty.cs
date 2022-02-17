namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes a property associated with an event message.
    /// </summary>
    public interface IEventProperty {

        /// <summary>
        /// Gets the event property name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the event value.
        /// </summary>
        object? Value { get; }

    }

}
