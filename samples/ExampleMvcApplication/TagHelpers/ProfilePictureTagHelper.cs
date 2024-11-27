using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ExampleMvcApplication.TagHelpers {

    /// <summary>
    /// This tag helper helps to simplify the display of user profile pictures.
    /// </summary>
    public class ProfilePictureTagHelper : TagHelper {

        /// <summary>
        /// HTML encoder (used when adding CSS classes to the generated HTML).
        /// </summary>
        private readonly HtmlEncoder _htmlEncoder;

        /// <summary>
        /// The URL of the user's profile picture.
        /// </summary>
        public string? Url { get; set; }


        /// <summary>
        /// Creates a new <see cref="ProfilePictureTagHelper"/> object.
        /// </summary>
        /// <param name="htmlEncoder">
        ///   The HTML encoder.
        /// </param>
        public ProfilePictureTagHelper(HtmlEncoder htmlEncoder) {
            _htmlEncoder = htmlEncoder;
        }


        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output) {
            if (string.IsNullOrWhiteSpace(Url)) {
                // No URL specified; don't output anything.
                output.SuppressOutput();
                return;
            }

            output.TagName = "img";
            output.Attributes.SetAttribute("src", Url);

            output.AddClass("rounded", _htmlEncoder);
            output.AddClass("profile-picture", _htmlEncoder);
        }

    }
}
