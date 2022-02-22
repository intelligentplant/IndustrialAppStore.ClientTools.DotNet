using Microsoft.AspNetCore.Http;

namespace IntelligentPlant.IndustrialAppStore.AspNetCore {

    /// <summary>
    /// Middleware that adds headers from the <see cref="CustomHeadersProvider"/> service to every 
    /// response.
    /// </summary>
    internal class CustomHeadersMiddleware {

        /// <summary>
        /// The next middleware delegate.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// The <see cref="CustomHeadersProvider"/> to use.
        /// </summary>
        private readonly CustomHeadersProvider _provider;


        /// <summary>
        /// Creates a new <see cref="CustomHeadersMiddleware"/> object.
        /// </summary>
        /// <param name="next">
        ///   The next middleware delegate.
        /// </param>
        /// <param name="provider">
        ///   The <see cref="CustomHeadersProvider"/> to use.
        /// </param>
        public CustomHeadersMiddleware(RequestDelegate next, CustomHeadersProvider provider) {
            _next = next;
            _provider = provider;
        }


        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">
        ///   The <see cref="HttpContext"/> for the request.
        /// </param>
        /// <returns>
        ///   A <see cref="Task"/> that will process the request.
        /// </returns>
        public async Task InvokeAsync(HttpContext context) {
            foreach (var item in _provider.GetHeaders()) {
                context.Response.Headers.TryAdd(item.Key, item.Value);
            }

            await _next(context).ConfigureAwait(false);
        }

    }
}
