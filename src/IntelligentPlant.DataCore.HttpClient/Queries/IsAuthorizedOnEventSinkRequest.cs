using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class IsAuthorizedOnEventSinkRequest : EventSinkRequest {

        [Required]
        [MinLength(1)]
        public string[] RoleNames { get; set; } = default!;

    }
}
