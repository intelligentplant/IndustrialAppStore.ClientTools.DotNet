using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.IndustrialAppStore.Client.Queries {

    /// <summary>
    /// A request to debug a user for app usage.
    /// </summary>
    public class DebitUserRequest {

        /// <summary>
        /// The amount to debit.
        /// </summary>
        [Range(0, double.MaxValue)]
        public double DebitAmount { get; set; }

    }

}
