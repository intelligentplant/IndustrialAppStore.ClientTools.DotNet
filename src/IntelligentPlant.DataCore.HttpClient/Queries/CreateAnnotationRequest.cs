using System.ComponentModel.DataAnnotations;
using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class CreateAnnotationRequest {

        [Required]
        public string DataSourceName { get; set; }

        [Required]
        public Annotation Annotation { get; set; }

    }
}
