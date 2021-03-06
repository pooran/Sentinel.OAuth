﻿namespace Sentinel.Sample.Managers
{
    using Sentinel.OAuth.Core.Constants.Identity;
    using Sentinel.OAuth.Core.Interfaces.Identity;
    using Sentinel.OAuth.Core.Interfaces.Managers;
    using Sentinel.OAuth.Models.Identity;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Sentinel.OAuth.Core.Models;

    public class SimpleClientManager : IClientManager
    {
        public Task<CreateClientResult> CreateClient(string clientId, string name, string redirectUri)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Authenticates the client. Used when authenticating with the authorization_code grant type.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <returns>The client principal.</returns>
        public async Task<ISentinelPrincipal> AuthenticateClientAsync(string clientId, string redirectUri)
        {
            if (clientId == "NUnit" && redirectUri == "http://localhost")
            {
                return new SentinelPrincipal(new SentinelIdentity(AuthenticationType.OAuth, new SentinelClaim(JwtClaimType.Name, clientId)));
            }

            return SentinelPrincipal.Anonymous;
        }

        /// <summary>
        ///     Authenticates the client. Used when authenticating with the client_credentials grant type.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="scope">The redirect URI.</param>
        /// <returns>The client principal.</returns>
        public async Task<ISentinelPrincipal> AuthenticateClientAsync(string clientId, IEnumerable<string> scope)
        {
            if (clientId == "NUnit")
            {
                return new SentinelPrincipal(new SentinelIdentity(AuthenticationType.OAuth, new SentinelClaim(JwtClaimType.Name, clientId)));
            }

            return SentinelPrincipal.Anonymous;
        }

        public Task<ISentinelPrincipal> AuthenticateClientCredentialsAsync(BasicAuthenticationDigest digest)
        {
            return this.AuthenticateClientCredentialsAsync(digest.UserId, digest.Password);
        }

        /// <summary>Authenticates the client credentials using client id and secret.</summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <returns>The client principal.</returns>
        public async Task<ISentinelPrincipal> AuthenticateClientCredentialsAsync(string clientId, string clientSecret)
        {
            // Return an authenticated principal if the client secret matches the client id
            if (clientId == clientSecret)
            {
                return new SentinelPrincipal(new SentinelIdentity(AuthenticationType.OAuth, new SentinelClaim(JwtClaimType.Name, clientId)));
            }

            return SentinelPrincipal.Anonymous;
        }

        public Task<ISentinelPrincipal> AuthenticateClientWithSignatureAsync(SignatureAuthenticationDigest digest)
        {
            throw new System.NotImplementedException();
        }
    }
}