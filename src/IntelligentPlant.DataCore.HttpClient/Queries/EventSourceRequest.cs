using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class EventSourceRequest {

        [Required]
        public string EventSourceName { get; set; }

    }
}
