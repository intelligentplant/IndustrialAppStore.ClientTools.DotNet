using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class EventSinkRequest {

        [Required]
        public string EventSinkName { get; set; } = default!;

    }
}
