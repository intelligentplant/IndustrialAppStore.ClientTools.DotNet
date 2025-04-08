namespace IntelligentPlant.DataCore.Client.Model {

    /// <summary>
    /// <see cref="TagValueBuilder"/> allows easy construction of <see cref="TagValue"/> instances using a fluent interface.
    /// </summary>
    public class TagValueBuilder {

        /// <summary>
        /// The tag name.
        /// </summary>
        private string _tagName;

        /// <summary>
        /// The sample time.
        /// </summary>
        private DateTime? _utcSampleTime;

        /// <summary>
        /// The numeric value for the sample.
        /// </summary>
        private double _numericValue = double.NaN;

        /// <summary>
        /// The display value for the sample.
        /// </summary>
        private string? _displayValue;

        /// <summary>
        /// The sample quality.
        /// </summary>
        private TagValueStatus _status = TagValueStatus.Good;

        /// <summary>
        /// The units for the sample.
        /// </summary>
        private string? _units;

        /// <summary>
        /// Notes or additional information about the sample.
        /// </summary>
        private string? _notes;

        /// <summary>
        /// Error details for the sample, if applicable.
        /// </summary>
        private string? _error;

        /// <summary>
        /// Custom sample properties.
        /// </summary>
        private Dictionary<string, object>? _properties;


        /// <summary>
        /// Creates a new <see cref="TagValueBuilder"/> instance.
        /// </summary>
        /// <param name="tagName">
        ///   The name of the tag that the sample is being created for.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="tagName"/> is <see langword="null"/> or white space.
        /// </exception>
        /// <remarks>
        /// 
        /// <para>
        ///   The <see cref="TagValueBuilder"/> uses the following defaults when 
        ///   constructed using <see cref="TagValueBuilder(string)"/>:
        /// </para>
        /// 
        /// <list type="table">
        ///   <item>
        ///     <term>Timestamp</term>
        ///     <description><see cref="DateTime.UtcNow"/></description>
        ///   </item>
        ///   <item>
        ///     <term>Status</term>
        ///     <description><see cref="TagValueStatus.Good"/></description>
        ///   </item>
        /// </list>
        /// 
        /// </remarks>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public TagValueBuilder(string tagName) {
            WithTagName(tagName);
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


        /// <summary>
        /// Creates a new <see cref="TagValueBuilder"/> instance using an existing tag value.
        /// </summary>
        /// <param name="value">
        ///   The value to initialise the <see cref="TagValueBuilder"/> with.
        /// </param>
        /// <param name="includeProperties">
        ///   Specifies if the <see cref="TagValue.Properties"/> from the existing 
        ///   <paramref name="value"/> should be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        public TagValueBuilder(TagValue value, bool includeProperties = true) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }

            _tagName = value.TagName;
            _utcSampleTime = value.UtcSampleTime;
            _numericValue = value.NumericValue;
            _displayValue = value.TextValue;
            _status = value.Status;
            _units = value.Unit;
            _notes = value.Notes;
            _error = value.Error;

            if (includeProperties) {
                WithProperties(value.Properties);
            }
        }


        /// <summary>
        /// Sets the tag name for the sample.
        /// </summary>
        /// <param name="name">
        ///   The tag name.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="name"/> is <see langword="null"/> or white space.
        /// </exception>
        public TagValueBuilder WithTagName(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentOutOfRangeException(nameof(name), Resources.Error_TagNameIsRequired);
            }

            _tagName = name;
            return this;
        }


        /// <summary>
        /// Sets the timestamp for the sample.
        /// </summary>
        /// <param name="timestamp">
        ///   The timestamp.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <remarks>
        ///   
        /// <para>
        ///   The <paramref name="timestamp"/> will be converted to UTC before being assigned. 
        ///   Conversion is performed based on the <see cref="DateTime.Kind"/> for the <paramref name="timestamp"/>:
        /// </para>
        /// 
        /// <list type="table">
        ///   <item>
        ///     <term><see cref="DateTimeKind.Utc"/></term>
        ///     <description>No conversion required.</description>
        ///   </item>
        ///   <item>
        ///     <term><see cref="DateTimeKind.Local"/></term>
        ///     <description>Conversion performed using <see cref="DateTime.ToUniversalTime"/>.</description>
        ///   </item>
        ///   <item>
        ///     <term><see cref="DateTimeKind.Unspecified"/></term>
        ///     <description>
        ///       The <paramref name="timestamp"/> is assumed to be UTC. A new <see cref="DateTime"/> 
        ///       is constructed using <see cref="DateTime(long, DateTimeKind)"/> using the 
        ///       <see cref="DateTime.Ticks"/> for the <paramref name="timestamp"/> and a kind of 
        ///       <see cref="DateTimeKind.Utc"/>.
        ///     </description>
        ///   </item>
        /// </list>
        /// 
        /// </remarks>
        public TagValueBuilder WithTimestamp(DateTime timestamp) {
            switch (timestamp.Kind) {
                case DateTimeKind.Utc:
                    _utcSampleTime = timestamp;
                    break;
                case DateTimeKind.Local:
                    _utcSampleTime = timestamp.ToUniversalTime();
                    break;
                default:
                    _utcSampleTime = new DateTime(timestamp.Ticks, DateTimeKind.Utc);
                    break;
            }
            return this;
        }


        /// <summary>
        /// Sets the numeric value for the sample.
        /// </summary>
        /// <param name="value">
        ///   The numeric value.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        public TagValueBuilder WithNumericValue(double value) {
            _numericValue = value;
            return this;
        }


        /// <summary>
        /// Sets the display value (text value) for the sample.
        /// </summary>
        /// <param name="value">
        ///   The display value (text value) for the sample.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <remarks>
        /// 
        /// <para>
        ///   <strong>Display value guidelines:</strong>
        /// </para>
        /// 
        /// <list type="bullet">
        ///   <item>
        ///     For text tags, use <see cref="WithDisplayValue"/> to set the text value for the 
        ///     sample.
        ///   </item>
        ///   <item>
        ///     For digital tags, use <see cref="WithDisplayValue"/> to set the name of the digital 
        ///     state if applicable.
        ///   </item>
        ///   <item>
        ///     For analogue tags, <strong>do not use <see cref="WithDisplayValue"/></strong>. 
        ///     Formatting of numeric values is an application concern and needs to take the end 
        ///     user's localisation settings into account.
        ///   </item>
        ///   <item>
        ///     Don't use <see cref="WithDisplayValue"/> to set metadata about the sample (such as 
        ///     how a value was calculated); use <see cref="WithNotes"/>, <see cref="WithProperty"/>, 
        ///     <see cref="WithProperties(KeyValuePair{string, object}[])"/>, <see cref="WithProperties(IEnumerable{KeyValuePair{string, object}})"/>, 
        ///     <see cref="WithError(Exception)"/> or <see cref="WithError(string)"/> 
        ///     to convey metadata about a sample instead.
        ///   </item>
        /// </list>
        /// 
        /// </remarks>
        /// <seealso cref="WithNotes"/>
        /// <seealso cref="WithProperty"/>
        /// <seealso cref="WithProperties(KeyValuePair{string, object}[])"/>
        /// <seealso cref="WithProperties(IEnumerable{KeyValuePair{string, object}})"/>
        /// <seealso cref="WithError(Exception)"/>
        /// <seealso cref="WithError(string)"/>
        public TagValueBuilder WithDisplayValue(string value) {
            _displayValue = value;
            return this;
        }

        /// <summary>
        /// Sets the quality status for the sample.
        /// </summary>
        /// <param name="status">
        ///   The status for the sample.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        public TagValueBuilder WithStatus(TagValueStatus status) {
            _status = status;
            return this;
        }


        /// <summary>
        /// Sets the units for the sample.
        /// </summary>
        /// <param name="units">
        ///   The units for the sample.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <remarks>
        ///   <see cref="WithUnits"/> should only be called if the units for the sample differ 
        ///   from the units specified on the tag definition (for example if the tag normally 
        ///   returns deg C but deg F is being for this sample).
        /// </remarks>
        public TagValueBuilder WithUnits(string units) {
            _units = units;
            return this;
        }


        /// <summary>
        /// Sets the notes associated with the sample.
        /// </summary>
        /// <param name="notes">
        ///   The notes.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <remarks>
        ///   Use <see cref="WithNotes"/> to provide additional contextual information about the 
        ///   sample (for example how the value of the sample was computed).
        /// </remarks>
        public TagValueBuilder WithNotes(string notes) {
            _notes = notes;
            return this;
        }


        /// <summary>
        /// Sets the error information for the sample.
        /// </summary>
        /// <param name="error">
        ///   The error that occurred.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <remarks>
        /// 
        /// <para>
        ///   In addition to setting the error message for the sample, calling <see cref="WithError(Exception)"/> 
        ///   updates multiple items on the sample as follows:
        /// </para>
        /// 
        /// <list type="table">
        ///   <item>
        ///     <term>Numeric Value</term>
        ///     <description><see cref="double.NaN"/></description>
        ///   </item>
        ///   <item>
        ///     <term>Display Value</term>
        ///     <description><c>"&lt;ERROR&gt;"</c></description>
        ///   </item>
        ///   <item>
        ///     <term>Status</term>
        ///     <description><see cref="TagValueStatus.Bad"/></description>
        ///   </item>
        /// </list>
        /// 
        /// </remarks>
        /// <seealso cref="WithError(string)"/>
        public TagValueBuilder WithError(Exception error) => WithError(error?.Message);


        /// <summary>
        /// Sets the error information for the sample.
        /// </summary>
        /// <param name="errorDetails">
        ///   The details of the error that occurred.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <remarks>
        /// 
        /// <para>
        ///   In addition to setting the error message for the sample, calling <see cref="WithError(string)"/> 
        ///   updates multiple items on the sample as follows:
        /// </para>
        /// 
        /// <list type="table">
        ///   <item>
        ///     <term>Numeric Value</term>
        ///     <description><see cref="double.NaN"/></description>
        ///   </item>
        ///   <item>
        ///     <term>Display Value</term>
        ///     <description><see cref="Resources.TagValue_DisplayValue_Error"/></description>
        ///   </item>
        ///   <item>
        ///     <term>Status</term>
        ///     <description><see cref="TagValueStatus.Bad"/></description>
        ///   </item>
        /// </list>
        /// 
        /// </remarks>
        /// <seealso cref="WithError(Exception)"/>
        public TagValueBuilder WithError(string? errorDetails) {
            _numericValue = double.NaN;
            _displayValue = Resources.TagValue_DisplayValue_Error;
            _status = TagValueStatus.Bad;
            _error = errorDetails;
            return this;
        }


        /// <summary>
        /// Clears all custom properties from the sample.
        /// </summary>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        public TagValueBuilder ClearProperties() {
            _properties = null;
            return this;
        }


        private Dictionary<string, object> GetOrCreatePropertyDictionary() => _properties ??= new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);


        /// <summary>
        /// Sets a custom property on the sample.
        /// </summary>
        /// <param name="name">
        ///   The property name.
        /// </param>
        /// <param name="value">
        ///   The property value.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="name"/> is <see langword="null"/> or white space.
        /// </exception>
        /// <seealso cref="WithProperties(KeyValuePair{string, object}[])"/>
        /// <seealso cref="WithProperties(IEnumerable{KeyValuePair{string, object}})"/>
        public TagValueBuilder WithProperty(string name, object value) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentOutOfRangeException(nameof(name), Resources.Error_PropertyNameIsRequired);
            }

            GetOrCreatePropertyDictionary()[name] = value;
            return this;
        }


        /// <summary>
        /// Sets custom properties on the sample.
        /// </summary>
        /// <param name="properties">
        ///   The properties.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <remarks>
        ///   Any items in <paramref name="properties"/> with a <see langword="null"/> or white 
        ///   space <see cref="KeyValuePair{TKey, TValue}.Key"/> will be ignored.
        /// </remarks>
        /// <seealso cref="WithProperty(string, object)"/>
        /// <seealso cref="WithProperties(IEnumerable{KeyValuePair{string, object}})"/>
        public TagValueBuilder WithProperties(params KeyValuePair<string, object>[] properties) => WithProperties((IEnumerable<KeyValuePair<string, object>>) properties);


        /// <summary>
        /// Sets custom properties on the sample.
        /// </summary>
        /// <param name="properties">
        ///   The properties.
        /// </param>
        /// <returns>
        ///   The <see cref="TagValueBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="properties"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   Any entry in <paramref name="properties"/> has a null or white space key.
        /// </exception>
        /// <seealso cref="WithProperty(string, object)"/>
        /// <seealso cref="WithProperties(KeyValuePair{string, object}[])"/>
        public TagValueBuilder WithProperties(IEnumerable<KeyValuePair<string, object>> properties) {
            if (properties == null) {
                throw new ArgumentNullException(nameof(properties));
            }

            var propDict = GetOrCreatePropertyDictionary();

            foreach (var item in properties) {
                if (string.IsNullOrWhiteSpace(item.Key)) {
                    throw new ArgumentOutOfRangeException(nameof(item), Resources.Error_PropertyNameIsRequired);
                }
                propDict[item.Key] = item.Value;
            }

            return this;
        }


        /// <summary>
        /// Builds the <see cref="TagValue"/> instance.
        /// </summary>
        /// <returns>
        ///   A new <see cref="TagValue"/> instance.
        /// </returns>
        public TagValue Build() => new TagValue(_tagName, _utcSampleTime ?? DateTime.UtcNow, _numericValue, _displayValue, _status, _units, _notes, _error, _properties);

    }
}
