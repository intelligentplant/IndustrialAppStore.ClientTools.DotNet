namespace IntelligentPlant.IndustrialAppStore.Client.Model {

    /// <summary>
    /// Describes the result of a request to refund a transaction.
    /// </summary>
    public class RefundUserResponse {

        /// <summary>
        /// A flag indicating if the transaction was successfully refunded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The message associated with the refund response.
        /// </summary>
        public string Message { get; set; }

    }

}
