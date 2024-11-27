using System.ComponentModel.DataAnnotations;


namespace IntelligentPlant.DataCore.Client.Model.Queries {

    /// <summary>
    /// A request to call a custom function.
    /// </summary>
    public class CustomFunctionRequest {

        /// <summary>
        /// Gets or sets the component name to send the custom function to. Leave blank for 
        /// general-purpose custom function messages.
        /// </summary>
        public string? ComponentName { get; set; }

        /// <summary>
        /// The name of the custom function to call.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string MethodName { get; set; } = default!;

        /// <summary>
        /// The function parameters.
        /// </summary>
        public IDictionary<string, string>? Parameters { get; set; }

    }
}
