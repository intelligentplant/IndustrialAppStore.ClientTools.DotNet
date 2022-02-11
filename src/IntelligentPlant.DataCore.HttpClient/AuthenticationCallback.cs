using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IntelligentPlant.DataCore.Client {

    /// <summary>
    /// Delegate used by message handlers created by <see cref="DataCoreHttpClient.CreateAuthenticationMessageHandler"/> 
    /// to create the <c>Authorize</c> header to attach to an outgoing request.
    /// </summary>
    /// <typeparam name="TContext">
    ///   The type of the context object passed to the <see cref="DataCoreHttpClient"/> method.
    /// </typeparam>
    /// <param name="request">
    ///   The outgoing HTTP request.
    /// </param>
    /// <param name="context">
    ///   The context value passed to the <see cref="DataCoreHttpClient"/> method.
    /// </param>
    /// <param name="cancellationToken">
    ///   The cancellation token for the operation.
    /// </param>
    /// <returns>
    ///   A task that will return the <see cref="AuthenticationHeaderValue"/> to set on the 
    ///   <c>Authorize</c> header in the outgoing request. If the return value is 
    ///   <see langword="null"/>, no <c>Authorize</c> header will be set.
    /// </returns>
    public delegate Task<AuthenticationHeaderValue?> AuthenticationCallback<TContext>(
        HttpRequestMessage request, 
        TContext? context, 
        CancellationToken cancellationToken
    );

}
