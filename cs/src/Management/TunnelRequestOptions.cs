// <copyright file="TunnelRequestOptions.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Microsoft.VsSaaS.TunnelService.Contracts;

namespace Microsoft.VsSaaS.TunnelService
{
    /// <summary>
    /// Options for tunnel service requests.
    /// </summary>
    /// <remarks>
    /// All options are disabled by default. In general, enabling options may result
    /// in slower queries (because the server has more work to do).
    /// 
    /// Certain options may only apply to certain kinds of requests.
    /// </remarks>
    public class TunnelRequestOptions
    {
        /// <summary>
        /// Gets or sets a tunnel access token for the request.
        /// </summary>
        /// <remarks>
        /// Note this should not be a _user_ access token (such as AAD or GitHub); use the
        /// callback parameter to the <see cref="TunnelManagementClient"/> constructor to
        /// supply user access tokens.
        /// </remarks>
        public string? AccessToken { get; set; }

        /// <summary>
        /// Gets or sets additional headers to be included in the request.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>>? AdditionalHeaders { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether HTTP redirect responses will be
        /// automatically followed.
        /// </summary>
        /// <remarks>
        /// The default is true. If false, a redirect response will cause a
        /// <see cref="HttpRequestException"/> to be thrown, with the redirect target location
        /// in the exception data.
        /// <para/>
        /// The tunnel service often redirects requests to the "home" cluster of the requested
        /// tunnel, when necessary to fulfill the request.
        /// </remarks>
        public bool FollowRedirects { get; set; } = true;

        /// <summary>
        /// Gets or sets a flag that requests tunnel ports when retrieving a tunnel object.
        /// </summary>
        public bool IncludePorts { get; set; }

        /// <summary>
        /// Gets or sets an optional list of scopes that should be authorized when retrieving a
        /// tunnel or tunnel port object.
        /// </summary>
        /// <remarks>
        /// For service-to-service calls using forwarded client credentials, the scope(s) should
        /// match the action the client is requesting, such as "host" or "connect". This enables
        /// the calling service to ensure the client is specifically authorized for the supplied
        /// scope(s), rather than just any scopes permitted by the API.
        /// </remarks>
        public string[]? Scopes { get; set; }

        /// <summary>
        /// Gets or sets an optional list of token scopes that are requested when retrieving
        /// a tunnel or tunnel port object.
        /// </summary>
        public string[]? TokenScopes { get; set; }

        /// <summary>
        /// If true on a create or update request then upon a name conflict, attempt to rename the
        /// existing tunnel to null and give the name to the tunnel from the request.
        /// </summary>
        public bool ForceRename { get; set; }

        /// <summary>
        /// Converts tunnel request options to a query string for HTTP requests to the
        /// tunnel management API.
        /// </summary>
        internal string ToQueryString()
        {
            var queryOptions = new Dictionary<string, string>();

            if (IncludePorts)
            {
                queryOptions["includePorts"] = "true";
            }

            if (Scopes != null)
            {
                TunnelAccessControl.ValidateScopes(Scopes);
                queryOptions["scopes"] = string.Join(",", Scopes);
            }

            if (TokenScopes != null)
            {
                TunnelAccessControl.ValidateScopes(TokenScopes);
                queryOptions["tokenScopes"] = string.Join(",", TokenScopes);
            }

            if (ForceRename)
            {
                queryOptions["forceRename"] = "true";
            }

            return string.Join('&', queryOptions.Select(
                (o) => $"{o.Key}={HttpUtility.UrlEncode(o.Value)}"));
        }
    }
}
