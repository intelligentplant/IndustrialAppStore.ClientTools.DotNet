using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class DeleteAnnotationRequest {

        [Required]
        public string DataSourceName { get; set; } = default!;

        [Required]
        public AnnotationIdentifier Annotation { get; set; } = default!;

    }
}
