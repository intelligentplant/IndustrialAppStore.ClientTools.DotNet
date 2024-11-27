using System.ComponentModel.DataAnnotations;

namespace IntelligentPlant.DataCore.Client.Queries {
    public class GetTagsRequest : DataSourceRequest, IValidatableObject {

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public string[] TagNamesOrIds { get; set; } = default!;


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (TagNamesOrIds != null) {
                if (TagNamesOrIds.Any(x => x == null)) {
                    yield return new ValidationResult(Resources.Error_TagMapOrListCannotContainNullItems, new[] { nameof(TagNamesOrIds) });
                }
            }
        }
    }
}
