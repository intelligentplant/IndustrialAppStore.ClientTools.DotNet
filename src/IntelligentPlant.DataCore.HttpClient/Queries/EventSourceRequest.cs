using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class EventSourceRequest {

        [Required]
        public string EventSourceName { get; set; }

    }
}
