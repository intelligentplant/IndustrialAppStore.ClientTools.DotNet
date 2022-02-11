using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.IndustrialAppStore.Client.Queries {

    /// <summary>
    /// A request to refund a transaction.
    /// </summary>
    public class RefundUserRequest {

        /// <summary>
        /// The transaction reference.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string TransactionRef { get; set; } = default!;

    }

}
