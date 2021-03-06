﻿namespace Sentinel.OAuth.Core.Models
{
    using Sentinel.OAuth.Core.Interfaces.Managers;
    using Sentinel.OAuth.Core.Interfaces.Providers;
    using Sentinel.OAuth.Core.Interfaces.Repositories;
    using System;

    using Common.Logging;

    /// <summary>The Sentinel authorization server options used for controlling the authoriztion system behavior.</summary>
    public class SentinelAuthorizationServerOptions
    {
        /// <summary>The events.</summary>
        private SentinelAuthorizationServerEvents events;

        /// <summary>URL of the authorization code endpoint.</summary>
        private string authorizationCodeEndpointUrl;

        /// <summary>URL of the token endpoint.</summary>
        private string tokenEndpointUrl;

        /// <summary>URL of the identity endpoint.</summary>
        private string identityEndpointUrl;

        /// <summary>
        /// Initializes a new instance of the SentinelAuthorizationServerOptions class.
        /// </summary>
        public SentinelAuthorizationServerOptions()
        {
            // Set default options
            this.RequireSecureConnection = true;
            this.EnableSignatureAuthentication = false;
            this.EnableBasicAuthentication = false;
            this.AccessTokenLifetime = TimeSpan.FromHours(1);
            this.AuthorizationCodeLifetime = TimeSpan.FromMinutes(5);
            this.RefreshTokenLifetime = TimeSpan.FromDays(90);
            this.Realm = "Sentinel";
            this.MaximumClockSkew = TimeSpan.FromSeconds(300);

            this.authorizationCodeEndpointUrl = "/oauth/authorize";
            this.tokenEndpointUrl = "/oauth/token";
            this.identityEndpointUrl = "/openid/userinfo";
        }

        /// <summary>Gets the events.</summary>
        /// <value>The events.</value>
        public SentinelAuthorizationServerEvents Events => this.events ?? (this.events = new SentinelAuthorizationServerEvents());

        /// <summary>Gets or sets the logger.</summary>
        /// <value>The logger.</value>
        public ILog Logger { get; set; }

        /// <summary>Gets or sets the realm.</summary>
        /// <value>The realm.</value>
        public string Realm { get; set; }

        /// <summary>Gets or sets the maximum clock skew.</summary>
        /// <value>The maximum clock skew.</value>
        public TimeSpan MaximumClockSkew { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether API key authentication is enabled.
        /// </summary>
        /// <value>true if enable API key authentication, false if not.</value>
        public bool EnableSignatureAuthentication { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether basic authentication is enabled.
        /// </summary>
        /// <value>true if enable basic authentication, false if not.</value>
        public bool EnableBasicAuthentication { get; set; }

        /// <summary>Gets or sets a value indicating whether to require a secure connection.</summary>
        /// <value>true if a secure connection is required, false if not.</value>
        public bool RequireSecureConnection { get; set; }

        /// <summary>Gets or sets the access token lifetime.</summary>
        /// <value>The access token lifetime.</value>
        public TimeSpan AccessTokenLifetime { get; set; }

        /// <summary>Gets or sets the authorization code lifetime.</summary>
        /// <value>The authorization code lifetime.</value>
        public TimeSpan AuthorizationCodeLifetime { get; set; }

        /// <summary>Gets or sets the refresh token lifetime.</summary>
        /// <value>The refresh token lifetime.</value>
        public TimeSpan RefreshTokenLifetime { get; set; }

        /// <summary>Gets or sets URI of the issuer.</summary>
        /// <value>The issuer URI.</value>
        public Uri IssuerUri { get; set; }

        /// <summary>
        /// Gets or sets the user management provider. This is the class responsible for locating and
        /// validating users.
        /// </summary>
        /// <value>The user management provider.</value>
        public IUserManager UserManager { get; set; }

        /// <summary>
        /// Gets or sets the client management provider. This is the class responsible for locating and
        /// validating clients.
        /// </summary>
        /// <value>The client management provider.</value>
        public IClientManager ClientManager { get; set; }

        /// <summary>
        /// Gets or sets the token store. This is the class responsible for creating and validating
        /// tokens and authorization codes.
        /// </summary>
        /// <value>The token store.</value>
        public ITokenManager TokenManager { get; set; }

        /// <summary>Gets or sets the token provider.</summary>
        /// <value>The token provider.</value>
        public ITokenProvider TokenProvider { get; set; }

        /// <summary>Gets or sets the token repository.</summary>
        /// <value>The token repository.</value>
        public ITokenRepository TokenRepository { get; set; }

        /// <summary>Gets or sets the client repository.</summary>
        /// <value>The client repository.</value>
        public IClientRepository ClientRepository { get; set; }

        /// <summary>Gets or sets the user repository.</summary>
        /// <value>The user repository.</value>
        public IUserRepository UserRepository { get; set; }

        /// <summary>Gets or sets the user API key repository.</summary>
        /// <value>The user API key repository.</value>
        public IUserApiKeyRepository UserApiKeyRepository { get; set; }

        /// <summary>Gets or sets the principal provider.</summary>
        /// <value>The principal provider.</value>
        public IPrincipalProvider PrincipalProvider { get; set; }

        /// <summary>Gets or sets the token crypto provider.</summary>
        /// <value>The token crypto provider.</value>
        public ICryptoProvider TokenCryptoProvider { get; set; }

        /// <summary>Gets or sets the password crypto provider.</summary>
        /// <value>The password crypto provider.</value>
        public IPasswordCryptoProvider PasswordCryptoProvider { get; set; }

        /// <summary>Gets or sets the API key crypto provider.</summary>
        /// <value>The API key crypto provider.</value>
        public IAsymmetricCryptoProvider SignatureCryptoProvider { get; set; }

        /// <summary>Gets or sets URL of the authorization code endpoint.</summary>
        /// <remarks>There must be a page answering on this url that is capable of logging in the user.</remarks>
        /// <value>The authorization code endpoint URL.</value>
        public string AuthorizationCodeEndpointUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(this.authorizationCodeEndpointUrl))
                {
                    return this.authorizationCodeEndpointUrl.StartsWith("/")
                               ? this.authorizationCodeEndpointUrl
                               : $"/{this.authorizationCodeEndpointUrl}";
                }

                return string.Empty;
            }

            set
            {
                this.authorizationCodeEndpointUrl = value;
            }
        }

        /// <summary>Gets or sets URL of the token endpoint.</summary>
        /// <value>The token endpoint URL.</value>
        public string TokenEndpointUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(this.tokenEndpointUrl))
                {
                    return this.tokenEndpointUrl.StartsWith("/")
                               ? this.tokenEndpointUrl
                               : $"/{this.tokenEndpointUrl}";
                }

                return string.Empty;
            }

            set
            {
                this.tokenEndpointUrl = value;
            }
        }

        /// <summary>Gets or sets URL of the identity endpoint.</summary>
        /// <value>The identity endpoint URL.</value>
        public string IdentityEndpointUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(this.identityEndpointUrl))
                {
                    return this.identityEndpointUrl.StartsWith("/")
                               ? this.identityEndpointUrl
                               : $"/{this.identityEndpointUrl}";
                }

                return string.Empty;
            }

            set
            {
                this.identityEndpointUrl = value;
            }
        }
    }
}