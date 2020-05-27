using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelligentPlant.IndustrialAppStore.Client.Queries {
    public class RefundUserRequest {

        [Required]
        [MaxLength(200)]
        public string TransactionRef { get; set; }

    }
}
