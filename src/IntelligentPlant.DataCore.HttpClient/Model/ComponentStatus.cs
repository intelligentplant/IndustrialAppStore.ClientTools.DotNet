using System;
using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes the current status of a Data Core component.
    /// </summary>
    public class ComponentStatus {

        /// <summary>
        /// Gets the running status of the component.
        /// </summary>
        public ComponentRuntimeState RunningStatus { get; set; }

        /// <summary>
        /// Gets a flag indicating if the component is running.
        /// </summary>
        [Obsolete("Use RunningStatus to determine the state of the component.", false)]
        public bool IsInitialised {
            get { return RunningStatus == ComponentRuntimeState.Running; }
        }

        /// <summary>
        /// Gets a flag indicating if the component is disabled.
        /// </summary>
        [Obsolete("Use RunningStatus to determine the state of the component.", false)]
        public bool IsDisabled {
            get { return RunningStatus == ComponentRuntimeState.Disabled; }
        }

        /// <summary>
        /// Gets a flag specifying if the component is currently running in debug mode.
        /// </summary>
        /// <remarks>
        /// When in debug mode, components are expected to record additional debugging or diagnostic information 
        /// while running.
        /// </remarks>
        public bool DebugMode { get; set; }

        /// <summary>
        /// Gets the UTC startup time for the component.
        /// </summary>
        public DateTime UtcStartupTime { get; set; }

        /// <summary>
        /// Gets the uptime for the component.
        /// </summary>
        public TimeSpan Uptime {
            get { return UtcStartupTime == DateTime.MinValue ? TimeSpan.Zero : DateTime.UtcNow - UtcStartupTime; }
        }

        /// <summary>
        /// Gets a collection of messages associated with the current status.
        /// </summary>
        public IList<LogMessage> Messages { get; set; }

        /// <summary>
        /// Gets driver-specific properties or state information about the component.
        /// </summary>
        public IEnumerable<NamedValue<string>> Properties { get; set; }

        /// <summary>
        /// Gets or sets the health status of the component.
        /// </summary>
        public ComponentHealth HealthStatus { get; set; }

    }
}
