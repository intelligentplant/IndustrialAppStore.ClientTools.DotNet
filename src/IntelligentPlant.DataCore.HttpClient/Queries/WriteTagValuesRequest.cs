using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using IntelligentPlant.DataCore.Client.Model;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class WriteTagValuesRequest : DataSourceRequest, IValidatableObject {

        [Required]
        [MinLength(1)]
        public TagValue[] Values { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (Values != null) {
                if (Values.Any(x => x == null)) {
                    yield return new ValidationResult(Resources.Error_WriteValuesCannotContainNullItems, new[] { nameof(Values) });
                }
            }
        }
    }
}
