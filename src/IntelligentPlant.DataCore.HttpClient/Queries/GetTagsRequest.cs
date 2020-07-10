using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class GetTagsRequest : DataSourceRequest, IValidatableObject {

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string[] TagNamesOrIds { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (TagNamesOrIds != null) {
                if (TagNamesOrIds.Any(x => x == null)) {
                    yield return new ValidationResult(Resources.Error_TagMapOrListCannotContainNullItems, new[] { nameof(TagNamesOrIds) });
                }
            }
        }
    }
}
