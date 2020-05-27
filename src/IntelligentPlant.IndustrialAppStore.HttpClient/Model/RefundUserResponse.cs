using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentPlant.IndustrialAppStore.Client.Model {
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
