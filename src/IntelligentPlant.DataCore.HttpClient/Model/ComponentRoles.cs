using System;
using System.Collections.Generic;

namespace IntelligentPlant.DataCore.Client.Model {
    /// <summary>
    /// Describes the roles a user has on a datasource.
    /// </summary>
    public class ComponentRoles : Dictionary<string, bool>
    {
        /// <summary>
        /// Creates a new <see cref="ComponentRoles"/> object that uses case-insensitive indexing.
        /// </summary>
        public ComponentRoles() : base(StringComparer.OrdinalIgnoreCase) { }


        /// <summary>
        /// Creates a new <see cref="ComponentRoles"/> that uses the specified index comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use for indexing.</param>
        public ComponentRoles(IEqualityComparer<string> comparer) : base(comparer) { }
    }
}
