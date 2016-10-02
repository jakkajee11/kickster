using KickSter.Api.Helpers;
using KickSter.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace KickSter.Api.Filters
{
    public class CustomAuthenticationFilterAttribute : Attribute, IAuthenticationFilter
    {        

        public bool AllowMultiple
        {
            get
            {
                return false;
            }
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // look for request
            var request = context.Request;
            // get authentication header
            IEnumerable<string> clientIdContainer = new List<string>();
            IEnumerable<string> domainContiner = new List<string>();
            //var tryGetClientId = request.Headers.TryGetValues("x-custom-ks-client-id", out clientIdContainer);
            //var tryGetDomain = request.Headers.TryGetValues("x-custom-ks-domain", out domainContiner);
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            //if (!tryGetClientId || !tryGetDomain)
            //{
            //    context.ErrorResult = new AuthenticationFailureResult("Unknown client", request);
            //    return;
            //} else
            //{
            //    var isClientExist = await AuthenticateAsync(clientIdContainer.FirstOrDefault(), domainContiner.FirstOrDefault());
            //    if (!isClientExist)
            //    {
            //        context.ErrorResult = new AuthenticationFailureResult("Invalid client", request);
            //        return;
            //    }
            //}
            //// if there is no authentication header, set the error result.
            //if (authorization == null)
            //{
            //    //context.ErrorResult = new AuthenticationFailureResult("Missing header", request);
            //    return;
            //}

            //// if not bearer authentication scheme, set the error result.
            //if (!authorization.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            //{
            //    //context.ErrorResult = new AuthenticationFailureResult("Authentication scheme not accepted", request);
            //    return;
            //}

            //// If the credentials are bad, set the error result.
            //if (string.IsNullOrEmpty(authorization.Parameter))
            //{
            //    //context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
            //    return;
            //}

            //IPrincipal principal = await AuthenticateAsync(authorization.Parameter, cancellationToken);
            //if (principal == null)
            //{
            //    context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);
            //}
            //// If the credentials are valid, set principal.
            //else
            //{
            //    context.Principal = principal;
            //}
            IPrincipal principal = new GenericPrincipal(new GenericIdentity("webuser"), new string[] { "user" });
            context.Principal = principal;
            Debug.WriteLine(string.Format("principal.Name {0}", principal.Identity.Name));
            //context.Principal = principal;
        }

        public async Task<IPrincipal> AuthenticateAsync(string token, CancellationToken cancellationToken)
        {
            var user = await AuthenticationHelper.FindUser(token);
            if (user == null) return null;
            return new GenericPrincipal(new GenericIdentity(user.NickName, "CustomIdentification"), new string[] { user.Role == null ? "user" : user.Role });
        }

        public async Task<bool> AuthenticateAsync(string clientId, string domain)
        {
            return await AuthenticationHelper.IsClientExist(clientId, domain);            
        }

        private string ExtractToken(string parameter)
        {
            Debug.WriteLine("parameter: " + parameter);
            var tokens = parameter.Split(new char[] { ' ' });
            return tokens.Count() == 0 ? null : tokens[0];
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Bearer");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            await Task.FromResult(0);
        }
    }
}