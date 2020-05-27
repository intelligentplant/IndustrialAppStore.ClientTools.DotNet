namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Describes the type of recalculation trigger used by a script tag.
    /// </summary>
    public enum ScriptTagTriggerType {

        /// <summary>
        /// The trigger type is unspecified, and should be inferred based on the script tag's 
        /// configuration i.e. if a valid schedule is defined, the trigger type should be 
        /// treated as <see cref="Schedule"/>.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The script tag should recalculate when the value of one or more of its tag references 
        /// changes.
        /// </summary>
        ValueChange = 1,

        /// <summary>
        /// The script tag should recalculate on a schedule.
        /// </summary>
        Schedule = 2

    }
}
