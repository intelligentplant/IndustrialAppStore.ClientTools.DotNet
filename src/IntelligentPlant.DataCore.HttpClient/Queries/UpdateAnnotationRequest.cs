using System.ComponentModel.DataAnnotations;

using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class UpdateAnnotationRequest {

        [Required]
        public string DataSourceName { get; set; }

        [Required]
        public AnnotationUpdate Annotation { get; set; }

    }
}
