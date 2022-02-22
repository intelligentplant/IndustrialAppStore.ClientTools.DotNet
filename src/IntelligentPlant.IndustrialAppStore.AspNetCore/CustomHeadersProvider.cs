using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace IntelligentPlant.IndustrialAppStore.AspNetCore {

    /// <summary>
    /// Service that provides custom headers to add to HTTP responses.
    /// </summary>
    internal class CustomHeadersProvider : IDisposable {

        /// <summary>
        /// <see cref="IConfiguration"/> section to load custom headers from.
        /// </summary>
        public const string ConfigurationSectionName = "CustomHeaders";

        /// <summary>
        /// Indicates if the services has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The <see cref="IConfiguration"/> to load headers from.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// <see cref="ChangeToken"/> registration for monitoring <see cref="_configuration"/> for 
        /// changes.
        /// </summary>
        private readonly IDisposable _ctReg;

        /// <summary>
        /// The custom headers.
        /// </summary>
        private Dictionary<string, string>? _headers;

        /// <summary>
        /// Lock for accessing <see cref="_headers"/>.
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();


        /// <summary>
        /// Creates a new <see cref="CustomHeadersProvider"/> object.
        /// </summary>
        /// <param name="configuration">
        ///   The <see cref="IConfiguration"/> to load custom headers from.
        /// </param>
        public CustomHeadersProvider(IConfiguration configuration) {
            _configuration = configuration;
            LoadHeaders();
            _ctReg = ChangeToken.OnChange(() => _configuration.GetReloadToken(), () => LoadHeaders());
        }


        /// <summary>
        /// Loads the custom headers from the <see cref="_configuration"/>.
        /// </summary>
        private void LoadHeaders() {
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var config = _configuration.GetSection(ConfigurationSectionName);
            if (config == null) {
                return;
            }

            foreach (var item in config.GetChildren()) {
                headers[item.Key] = item.Value;
            }

            _lock.EnterWriteLock();
            try {
                _headers = headers;
            }
            finally {
                _lock.ExitWriteLock();
            }
        }


        /// <summary>
        /// Gets the custom headers to add to an HTTP response.
        /// </summary>
        /// <returns>
        ///   The custom headers.
        /// </returns>
        public IEnumerable<KeyValuePair<string, string>> GetHeaders() {
            _lock.EnterReadLock();
            try {
                return _headers?.ToArray() ?? Array.Empty<KeyValuePair<string, string>>();
            }
            finally {
                _lock.ExitReadLock();
            }
        }


        /// <inheritdoc/>
        public void Dispose() {
            if (_disposed) {
                return;
            }

            _ctReg.Dispose();
            _lock.Dispose();
            _headers?.Clear();
            _disposed = true;
        }
    }
}
