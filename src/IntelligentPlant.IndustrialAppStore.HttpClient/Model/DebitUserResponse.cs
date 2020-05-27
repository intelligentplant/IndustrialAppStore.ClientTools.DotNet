
namespace IntelligentPlant.IndustrialAppStore.Client.Model {

    /// <summary>
    /// Describes the result of a call to debit a user for app usage.
    /// </summary>
    public class DebitUserResponse {

        /// <summary>
        /// The transaction ID for the debit request.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// A flag indicating if the transaction was successfully processed.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The message associated with the transaction.
        /// </summary>
        public string Message { get; set; }

    }
}
