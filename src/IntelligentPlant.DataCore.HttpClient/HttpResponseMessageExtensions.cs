using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// Extensions for <see cref="HttpResponseMessage"/>.
    /// </summary>
    public static class HttpResponseMessageExtensions {

        /// <summary>
        /// Throws a <see cref="DataCoreHttpClientException"/> if the response returned a non-good 
        /// status code.
        /// </summary>
        /// <param name="response">
        ///   The response.
        /// </param>
        /// <returns>
        ///   A task that will throw a <see cref="DataCoreHttpClientException"/> if the response 
        ///   returned a non-good status code.
        /// </returns>
        public static async Task ThrowOnErrorResponse(this HttpResponseMessage response) {
            if (response == null) {
                throw new ArgumentNullException(nameof(response));
            }
            if (response.IsSuccessStatusCode) {
                return;
            }

            var msg = string.Format(Resources.Error_DefaultHttpErrorMessage, response.RequestMessage.Method.Method, response.RequestMessage.RequestUri, (int) response.StatusCode, response.ReasonPhrase);
            throw await DataCoreHttpClientException.FromHttpResponseMessage(msg, response).ConfigureAwait(false);
        }

    }
}
