namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// Describes the validation result for an expression.
    /// </summary>
    public class ExpressionValidationResult {

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        public string Expression { get; set; } = default!;

        /// <summary>
        /// Gets or sets a flay stating if the expression is valid or not.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// The validation errors that were detected.
        /// </summary>
        private ICollection<string> _validationErrors = new List<string>();

        /// <summary>
        /// Gets or sets the validation errors that were detected.
        /// </summary>
        public ICollection<string> ValidationErrors {
            get { return _validationErrors; }
            set { _validationErrors = new List<string>(value); }
        }

        /// <summary>
        /// The tag references found in <see cref="P:Expression"/>.
        /// </summary>
        private ISet<ExpressionTagReference> _references = new HashSet<ExpressionTagReference>();

        /// <summary>
        /// Gets or sets the tag references found in <see cref="Expression"/>.
        /// </summary>
        public ISet<ExpressionTagReference> References {
            get { return _references; }
            set { _references = new HashSet<ExpressionTagReference>(value); }
        }

    }

}
