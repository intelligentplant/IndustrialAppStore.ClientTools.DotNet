using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IntelligentPlant.DataCore.Client.Model.Scripting {

    /// <summary>
    /// Base class for script tag settings.
    /// </summary>
    public class ScriptTagSettingsBase: IValidatableObject {

        /// <summary>
        /// Gets or sets the tags that are referenced by the script tag. Up to <see cref="MaximumTagReferenceCount"/> 
        /// references can be specified. Each key can be up to <see cref="MaximumTagReferenceNameLength"/> 
        /// in length and must match <see cref="TagReferenceNameRegex"/>.
        /// </summary>
        public IDictionary<string, TagReference>? TagReferences { get; set; }

        /// <summary>
        /// Gets or sets the digital states for the script tag. Up to <see cref="MaximumDigitalStateCount"/> 
        /// states can be specified. Each key can be up to <see cref="MaximumDigitalStateNameLength"/> 
        /// in length.
        /// </summary>
        public IDictionary<string, int>? DigitalStates { get; set; }

        /// <summary>
        /// Gets or sets a flag the indicates if the tag is allowed to trigger or reset events.
        /// </summary>
        public bool CanRaiseEvents { get; set; }

        /// <summary>
        /// Gets or sets the definitions for the events that the tag is allowed to trigger or reset. Up 
        /// to <see cref="MaximumEventDefinitionCount"/> event definitions can be defined. Each key can 
        /// be up to <see cref="MaximumEventDefinitionNameLength"/> in length.
        /// </summary>
        public IDictionary<string, ScriptTagEventDefinition>? EventDefinitions { get; set; }

        /// <summary>
        /// Gets or sets additional properties for the tag.
        /// </summary>
        public IDictionary<string, string>? Properties { get; set; }

        /// <summary>
        /// Gets or sets the settings for the tag's value script.
        /// </summary>
        [Required]
        public ScriptTagValueScriptSettings ValueScript { get; set; } = default!;

        #region [ Fields/properties to assist with validation ]

        /// <summary>
        /// Maximum number of tag references that can be defined.
        /// </summary>
        public const int MaximumTagReferenceCount = 20;

        /// <summary>
        /// Maximum length of a tag reference name.
        /// </summary>
        public const int MaximumTagReferenceNameLength = 100;

        /// <summary>
        /// A regex that ensures that tag reference names are valid.
        /// </summary>
        public static Regex TagReferenceNameRegex { get; } = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

        /// <summary>
        /// Maximum number of digital states that can be defined.
        /// </summary>
        public const int MaximumDigitalStateCount = 20;

        /// <summary>
        /// Maximum length of a digital state name.
        /// </summary>
        public const int MaximumDigitalStateNameLength = 100;

        /// <summary>
        /// Maximum number of event definitions that can be defined.
        /// </summary>
        public const int MaximumEventDefinitionCount = 20;

        /// <summary>
        /// Maximum length of an event name.
        /// </summary>
        public const int MaximumEventDefinitionNameLength = 100;

        #endregion


        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <param name="validationContext">
        ///   The validation context.
        /// </param>
        /// <returns>
        ///   The validation errors.
        /// </returns>
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) {
            if (TagReferences != null) {
                if (TagReferences.Count > MaximumTagReferenceCount) {
                    yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Client.Resources.Error_Scripting_TooManyTagReferences, MaximumTagReferenceCount), new[] { nameof(TagReferences) });
                }

                foreach (var item in TagReferences) {
                    if (item.Key.Length > MaximumTagReferenceNameLength) {
                        yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Client.Resources.Error_Scripting_TagReferenceNameIsTooLong, MaximumTagReferenceNameLength, item.Key), new[] { nameof(TagReferences) });
                    }

                    if (!TagReferenceNameRegex.IsMatch(item.Key)) {
                        yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Client.Resources.Error_Scripting_TagReferenceNameIsInvalid, item.Key), new[] { nameof(TagReferences) });
                    }

                    if (item.Value == null) {
                        yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Client.Resources.Error_Scripting_TagReferenceValueCannotBeNull, item.Key), new[] { nameof(TagReferences) });
                    }
                }
            }

            if (DigitalStates != null) {
                if (DigitalStates.Count > MaximumDigitalStateCount) {
                    yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Client.Resources.Error_Scripting_TooManyDigitalStates, MaximumDigitalStateCount), new[] { nameof(DigitalStates) });
                }

                foreach (var item in DigitalStates.Keys) {
                    if (item.Length > MaximumDigitalStateNameLength) {
                        yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Client.Resources.Error_Scripting_DigitalStateNameIsTooLong, MaximumDigitalStateNameLength, item), new[] { nameof(DigitalStates) });
                    }
                }
            }
            if (EventDefinitions != null) {
                if (EventDefinitions.Count > MaximumEventDefinitionCount) {
                    yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Client.Resources.Error_Scripting_TooManyEventDefinitions, MaximumEventDefinitionCount), new[] { nameof(EventDefinitions) });
                }

                foreach (var item in EventDefinitions) {
                    if (item.Key.Length > MaximumEventDefinitionNameLength) {
                        yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Client.Resources.Error_Scripting_EventDefinitionNameIsTooLong, MaximumEventDefinitionNameLength, item.Key), new[] { nameof(EventDefinitions) });
                    }

                    if (item.Value == null) {
                        yield return new ValidationResult(String.Format(CultureInfo.CurrentCulture, Client.Resources.Error_Scripting_EventDefinitionValueCannotBeNull, item.Key), new[] { nameof(EventDefinitions) });
                    }
                }
            }

            foreach (var item in Validate(validationContext)) {
                yield return item;
            }
        }


        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// The validation errors.
        /// </returns>
        protected virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            return Array.Empty<ValidationResult>();
        }

    }
}
