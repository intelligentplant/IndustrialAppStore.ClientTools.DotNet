using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class DeleteAnnotationRequest {

        [Required]
        public string DataSourceName { get; set; }

        [Required]
        public AnnotationIdentifier Annotation { get; set; }

    }
}
