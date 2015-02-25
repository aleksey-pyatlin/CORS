// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Results returned by <see cref="ICorsEngine"/>.
    /// </summary>
    public class CorsResult
    {
        private long? _preflightMaxAge;

        /// <summary>
        /// Gets a value indicating whether the result is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return ErrorMessages.Count == 0;
            }
        }

        /// <summary>
        /// Gets the error messages.
        /// </summary>
        public IList<string> ErrorMessages { get; } = new List<string>();

        /// <summary>
        /// Gets or sets the allowed origin.
        /// </summary>
        public string AllowedOrigin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource supports user credentials.
        /// </summary>
        public bool SupportsCredentials { get; set; }

        /// <summary>
        /// Gets the allowed methods.
        /// </summary>
        public IList<string> AllowedMethods { get; } = new List<string>();

        /// <summary>
        /// Gets the allowed headers.
        /// </summary>
        public IList<string> AllowedHeaders { get; } = new List<string>();

        /// <summary>
        /// Gets the allowed headers that can be exposed on the response.
        /// </summary>
        public IList<string> AllowedExposedHeaders { get; } = new List<string>();

        /// <summary>
        /// Gets or sets the number of seconds the results of a preflight request can be cached.
        /// </summary>
        public long? PreflightMaxAge
        {
            get
            {
                return _preflightMaxAge;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.PreflightMaxAgeOutOfRange);
                }
                _preflightMaxAge = value;
            }
        }

        /// <summary>
        /// Returns CORS-specific headers that should be added to the response.
        /// </summary>
        /// <returns>The response headers.</returns>
        public virtual IDictionary<string, string> ToResponseHeaders()
        {
            var headers = new Dictionary<string, string>();

            if (AllowedOrigin != null)
            {
                headers.Add(CorsConstants.AccessControlAllowOrigin, AllowedOrigin);
            }

            if (SupportsCredentials)
            {
                headers.Add(CorsConstants.AccessControlAllowCredentials, "true");
            }

            if (AllowedMethods.Count > 0)
            {
                // Filter out simple methods
                var nonSimpleAllowMethods = AllowedMethods.Where(m =>
                    !CorsConstants.SimpleMethods.Contains(m, StringComparer.OrdinalIgnoreCase));
                AddHeader(headers, CorsConstants.AccessControlAllowMethods, nonSimpleAllowMethods);
            }

            if (AllowedHeaders.Count > 0)
            {
                // Filter out simple request headers
                var nonSimpleAllowRequestHeaders = AllowedHeaders.Where(header =>
                    !CorsConstants.SimpleRequestHeaders.Contains(header, StringComparer.OrdinalIgnoreCase));
                AddHeader(headers, CorsConstants.AccessControlAllowHeaders, nonSimpleAllowRequestHeaders);
            }

            if (AllowedExposedHeaders.Count > 0)
            {
                // Filter out simple response headers
                var nonSimpleAllowResponseHeaders = AllowedExposedHeaders.Where(header =>
                    !CorsConstants.SimpleResponseHeaders.Contains(header, StringComparer.OrdinalIgnoreCase));
                AddHeader(headers, CorsConstants.AccessControlExposeHeaders, nonSimpleAllowResponseHeaders);
            }

            if (PreflightMaxAge.HasValue)
            {
                headers.Add(CorsConstants.AccessControlMaxAge, PreflightMaxAge.ToString());
            }

            return headers;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("IsValid: ");
            builder.Append(IsValid);
            builder.Append(", AllowCredentials: ");
            builder.Append(SupportsCredentials);
            builder.Append(", PreflightMaxAge: ");
            builder.Append(PreflightMaxAge.HasValue ?
                PreflightMaxAge.Value.ToString(CultureInfo.InvariantCulture) : "null");
            builder.Append(", AllowOrigin: ");
            builder.Append(AllowedOrigin);
            builder.Append(", AllowExposedHeaders: {");
            builder.Append(string.Join(",", AllowedExposedHeaders));
            builder.Append("}");
            builder.Append(", AllowHeaders: {");
            builder.Append(string.Join(",", AllowedHeaders));
            builder.Append("}");
            builder.Append(", AllowMethods: {");
            builder.Append(string.Join(",", AllowedMethods));
            builder.Append("}");
            builder.Append(", ErrorMessages: {");
            builder.Append(string.Join(",", ErrorMessages));
            builder.Append("}");
            return builder.ToString();
        }

        private static void AddHeader(
            IDictionary<string, string> headers,
            string headerName,
            IEnumerable<string> headerValues)
        {
            string methods = string.Join(",", headerValues);
            if (!string.IsNullOrEmpty(methods))
            {
                headers.Add(headerName, methods);
            }
        }
    }
}