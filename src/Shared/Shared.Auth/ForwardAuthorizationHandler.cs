using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Shared.Auth;

public sealed class ForwardAuthorizationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authorization = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorization);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
