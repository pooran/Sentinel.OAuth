﻿namespace Sentinel.OAuth.Models.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;

    using Newtonsoft.Json;

    using Sentinel.OAuth.Converters;
    using Sentinel.OAuth.Core.Interfaces.Identity;

    /// <summary>A JSON-serializable identity.</summary>
    [DebuggerDisplay("AuthenticationType: {AuthenticationType}, IsAuthenticated: {IsAuthenticated}, Name: {Name}, Claims: {Claims.Count()}")]
    public class SentinelIdentity : ISentinelIdentity
    {
        /// <summary>The locker.</summary>
        private readonly object locker = new object();

        /// <summary>The name.</summary>
        private string name;

        /// <summary>The claims.</summary>
        private IEnumerable<ISentinelClaim> claims;

        /// <summary>Initializes a new instance of the <see cref="SentinelIdentity"/> class.</summary>
        /// <param name="authenticationType">The type of the authentication.</param>
        public SentinelIdentity(string authenticationType)
        {
            this.AuthenticationType = authenticationType;
            this.Claims = new List<ISentinelClaim>();
        }

        /// <summary>Initializes a new instance of the <see cref="SentinelIdentity"/> class.</summary>
        /// <exception cref="ArgumentNullException">
        /// Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="authenticationType">The type of the authentication.</param>
        /// <param name="identity">The identity.</param>
        public SentinelIdentity(string authenticationType, IIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            this.Name = identity.Name;
            this.AuthenticationType = authenticationType;

            var claimsIdentity = identity as ClaimsIdentity;

            if (claimsIdentity != null)
            {
                foreach (var claim in claimsIdentity.Claims)
                {
                    this.AddClaim(claim);
                }
            }

            var sentinelIdentity = identity as SentinelIdentity;

            if (sentinelIdentity != null)
            {
                foreach (var claim in sentinelIdentity.Claims)
                {
                    this.AddClaim(claim);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SentinelIdentity"/> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public SentinelIdentity(IIdentity identity)
            : this(identity.AuthenticationType, identity)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the SentinelIdentity class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="authenticationType">The type of the authentication.</param>
        /// <param name="claims">The claims.</param>
        public SentinelIdentity(string authenticationType, params ISentinelClaim[] claims)
        {
            if (claims == null)
            {
                throw new ArgumentNullException("claims");
            }

            this.AuthenticationType = authenticationType;
            this.Claims = new List<ISentinelClaim>();

            foreach (var claim in claims)
            {
                this.AddClaim(claim);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the SentinelIdentity class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown when one or more required arguments are null.
        /// </exception>
        /// <param name="authenticationType">The type of the authentication.</param>
        /// <param name="claims">The claims.</param>
        public SentinelIdentity(string authenticationType, IEnumerable<ISentinelClaim> claims)
            : this(authenticationType, claims.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the SentinelIdentity class.
        /// </summary>
        /// <param name="authenticationType">The type of the authentication.</param>
        /// <param name="claims">The claims.</param>
        public SentinelIdentity(string authenticationType, IEnumerable<Claim> claims)
            : this(authenticationType, claims.Select(x => new SentinelClaim(x)))
        {
        }

        /// <summary>
        /// Prevents a default instance of the SentinelIdentity
        /// class from being created.
        /// </summary>
        [JsonConstructor]
        private SentinelIdentity()
        {
        }

        /// <summary>Gets an unauthorized/anonymous Sentinel identity object.</summary>
        /// <value>An unauthorized/anonymous Sentinel identity object.</value>
        public static ISentinelIdentity Anonymous
        {
            get
            {
                return new SentinelIdentity();
            }
        }

        /// <summary>
        /// Gets the type of the authentication.
        /// </summary>
        /// <value>The type of the authentication.</value>
        [JsonProperty]
        public string AuthenticationType { get; private set; }

        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <value>The claims.</value>
        [JsonProperty]
        [JsonConverter(typeof(SentinelClaimConverter))]
        public IEnumerable<ISentinelClaim> Claims
        {
            get
            {
                return this.claims ?? (this.claims = new List<ISentinelClaim>());
            }

            private set
            {
                this.claims = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// For this value to be true, both AuthenticationType and Name must set to a non-null value.
        /// </summary>
        /// <value><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</value>
        public bool IsAuthenticated
        {
            get
            {
                return !string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.AuthenticationType);
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty]
        public string Name
        {
            get
            {
                // Get name from claims if not specified
                if (string.IsNullOrEmpty(this.name))
                {
                    var nameClaim = this.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

                    if (nameClaim != null)
                    {
                        this.name = nameClaim.Value;
                    }
                }

                return this.name;
            }

            private set
            {
                this.name = value;
            }
        }

        /// <summary>Adds a claim.</summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        public void AddClaim(string type, string value)
        {
            lock (this.locker)
            {
                var c = this.Claims.ToList();
                c.Add(new SentinelClaim(type, value));

                this.Claims = c;
            }
        }

        /// <summary>
        /// Adds the claims.
        /// </summary>
        /// <param name="claims">The claims.</param>
        public void AddClaim(params Claim[] claims)
        {
            lock (this.locker)
            {
                var c = this.Claims.ToList();
                c.AddRange(claims.Select(claim => (SentinelClaim)claim));

                this.Claims = c;
            }
        }

        /// <summary>Adds the claims.</summary>
        /// <param name="claims">The claims.</param>
        public void AddClaim(params ISentinelClaim[] claims)
        {
            lock (this.locker)
            {
                var c = this.Claims.ToList();
                c.AddRange(claims);

                this.Claims = c;
            }
        }

        /// <summary>Removes the claim matching the expression.</summary>
        /// <param name="expression">The expression.</param>
        public void RemoveClaim(Func<ISentinelClaim, bool> expression)
        {
            lock (this.locker)
            {
                var claim = this.Claims.FirstOrDefault(expression);

                if (claim != null)
                {
                    var claims = this.Claims.ToList();
                    claims.Remove(claim);

                    this.Claims = claims;
                }
            }
        }

        /// <summary>Converts this object to a JSON string.</summary>
        /// <returns>This object as a JSON string.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>Runs the specified expression against the claimset and returns true if it contains a claim matching the predicate.</summary>
        /// <param name="expression">The expression.</param>
        /// <returns><c>true</c> if the claim exists, <c>false</c> if not.</returns>
        public bool HasClaim(Func<ISentinelClaim, bool> expression)
        {
            return this.Claims.Any(expression);
        }

        /// <summary>
        /// Checks if this identity contains any claims matching the specified type and value.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the claim exists, <c>false</c> if not.</returns>
        public bool HasClaim(string type, string value)
        {
            return this.Claims.Any(x => x.Type == type && x.Value == value);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return
                string.Format(
                    "AuthenticationType: {0}, IsAuthenticated: {1}, Name: {2}, Claims: [{3}]",
                    this.AuthenticationType,
                    this.IsAuthenticated,
                    this.Name,
                    this.Claims != null ? string.Join(", ", this.Claims) : string.Empty);
        }
    }
}