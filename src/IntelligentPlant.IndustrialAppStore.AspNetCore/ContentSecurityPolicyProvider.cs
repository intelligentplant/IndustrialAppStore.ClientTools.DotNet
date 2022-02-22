using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace IntelligentPlant.IndustrialAppStore.AspNetCore {

    /// <summary>
    /// Service that defines the default content security policy provider to add to HTTP responses.
    /// </summary>
    public class ContentSecurityPolicyProvider : IDisposable {

        /// <summary>
        /// <see cref="IConfiguration"/> section to load the default policy from.
        /// </summary>
        public const string ConfigurationSectionName = "ContentSecurityPolicy";

        /// <summary>
        /// Indicates if the services has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The logger for the provider.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Logs when policies are loaded or reloaded.
        /// </summary>
        private static readonly Action<ILogger, Exception?> s_logPoliciesLoaded = LoggerMessage.Define(LogLevel.Debug, new EventId(1, "PoliciesLoaded"), Resources.Log_ContentSecurityPoliciesLoaded);
        
        /// <summary>
        /// Logs when a policy is applied.
        /// </summary>
        private static readonly Action<ILogger, string, int, Exception?> s_logPolicyApplied = LoggerMessage.Define<string, int>(LogLevel.Trace, new EventId(2, "PolicyApplied"), Resources.Log_ContentSecurityPolicyApplied);

        /// <summary>
        /// The <see cref="IConfiguration"/> to load the default policy from.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// <see cref="ChangeToken"/> registration for monitoring <see cref="_configuration"/> for 
        /// changes.
        /// </summary>
        private readonly IDisposable _ctReg;

        /// <summary>
        /// The policy directives.
        /// </summary>
        private (string Name, int Priority, Func<string, bool> IsMatch, IDictionary<string, string[]> DefaultPolicy)[]? _policies;

        /// <summary>
        /// Lock for accessing <see cref="_policies"/>.
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// When <see langword="true"/>, the CSP should only be used to generate violation reports.
        /// </summary>
        private bool _reportOnly;


        /// <summary>
        /// Creates a new <see cref="ContentSecurityPolicyProvider"/> object.
        /// </summary>
        /// <param name="configuration">
        ///   The <see cref="IConfiguration"/> to load the default content security policy from.
        /// </param>
        /// <param name="logger">
        ///   The logger for the provider.
        /// </param>
        public ContentSecurityPolicyProvider(IConfiguration configuration, ILogger<ContentSecurityPolicyProvider> logger) {
            _configuration = configuration;
            _logger = logger;

            LoadPolicies();
            _ctReg = ChangeToken.OnChange(() => _configuration.GetReloadToken(), () => LoadPolicies());
        }


        /// <summary>
        /// Loads the default content security policies from the <see cref="_configuration"/>.
        /// </summary>
        private void LoadPolicies() {
            var options = new ContentSecurityPolicyOptions();
            _configuration.GetSection(ConfigurationSectionName).Bind(options);

            try {
                if (options.Policies == null || options.Policies.Count == 0) {
                    _lock.EnterWriteLock();
                    try {
                        _reportOnly = false;
                        _policies = Array.Empty<(string, int, Func<string, bool>, IDictionary<string, string[]>)>();
                        return;
                    }
                    finally {
                        _lock.ExitWriteLock();
                    }
                }

                var policies = new List<(string Name, int Priority, Func<string, bool> IsMatch, IDictionary<string, string[]> Directives)>();
                foreach (var item in options.Policies.OrderByDescending(x => x.Value.Priority)) {
                    if (item.Value?.Policy == null) {
                        continue;
                    }

                    Func<string, bool> isMatch;
                    if (item.Value.Match == null) {
                        isMatch = s => true;
                    }
                    else {
                        var regexList = item.Value.Match.Select(x => new Regex(string.Concat("^", Regex.Escape(x).Replace(@"\*", ".*"), "$"))).ToArray();
                        isMatch = s => regexList.Any(x => x.IsMatch(s));
                    }

                    policies.Add((item.Key, item.Value.Priority, isMatch, item.Value.Policy));
                }

                _lock.EnterWriteLock();
                try {
                    _reportOnly = options.ReportOnly;
                    _policies = policies.ToArray();
                }
                finally {
                    _lock.ExitWriteLock();
                }
            }
            finally {
                s_logPoliciesLoaded(_logger, null);
            }
        }


        /// <summary>
        /// Creates a <see cref="ContentSecurityPolicyBuilder"/> for the specified path.
        /// </summary>
        /// <param name="path">
        ///   The path.
        /// </param>
        /// <returns>
        ///   The default content security policy directives.
        /// </returns>
        public ContentSecurityPolicyBuilder CreatePolicyBuilder(PathString path) {
            var builder = new ContentSecurityPolicyBuilder();

            _lock.EnterReadLock();
            try {
                builder.ReportOnly = _reportOnly;

                if (_policies != null) {
                    var p = path.ToString();

                    foreach (var policy in _policies) {
                        if (!policy.IsMatch(p)) {
                            continue;
                        }

                        foreach (var item in policy.DefaultPolicy) {
                            var policyDirective = builder.GetOrCreateDirective(item.Key);
                            foreach (var value in item.Value) {
                                if (string.IsNullOrWhiteSpace(value)) {
                                    continue;
                                }

                                if (value.StartsWith("-:", StringComparison.Ordinal)) {
                                    // Remove a value from the policy directive.
                                    if (value.Length == 2) {
                                        continue;
                                    }

                                    policyDirective.Remove(value.Substring(2));
                                }
                                else if (value.StartsWith("+:", StringComparison.Ordinal)) {
                                    // Add a value to the policy directive.
                                    if (value.Length == 2) {
                                        continue;
                                    }

                                    policyDirective.Add(value.Substring(2));
                                }
                                else {
                                    // Treat this as a value to add to the policy directive.
                                    policyDirective.Add(value);
                                }
                            }
                        }

                        s_logPolicyApplied(_logger, policy.Name, policy.Priority, null);
                    }
                }
            }
            finally {
                _lock.ExitReadLock();
            }

            return builder;
        }


        /// <inheritdoc/>
        public void Dispose() {
            if (_disposed) {
                return;
            }

            _ctReg.Dispose();
            _lock.Dispose();
            _disposed = true;
        }
    }
}
