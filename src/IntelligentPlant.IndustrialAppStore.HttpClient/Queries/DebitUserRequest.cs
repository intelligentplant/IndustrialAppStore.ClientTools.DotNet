using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelligentPlant.IndustrialAppStore.Client.Queries {
    public class DebitUserRequest {

        [Range(0, double.MaxValue)]
        public double DebitAmount { get; set; }

    }
}
