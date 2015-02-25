// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Defines the policy for Cross-Origin requests based on the CORS specifications.
    /// </summary>
    public class CorsPolicy
    {
        private long? _preflightMaxAge;

        /// <summary>
        /// Gets or sets a value indicating whether to allow all headers.
        /// </summary>
        public bool AllowAnyHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow all methods.
        /// </summary>
        public bool AllowAnyMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow all origins.
        /// </summary>
        public bool AllowAnyOrigin
        {
            get
            {
                if (Origins == null || Origins.Count != 1 || Origins.Count == 1 && Origins[0] != "*")
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the headers that the resource might use and can be exposed.
        /// </summary>
        public IList<string> ExposedHeaders { get; } = new List<string>();

        /// <summary>
        /// Gets the headers that are supported by the resource.
        /// </summary>
        public IList<string> Headers { get; } = new List<string>();

        /// <summary>
        /// Gets the methods that are supported by the resource.
        /// </summary>
        public IList<string> Methods { get; } = new List<string>();

        /// <summary>
        /// Gets the origins that are allowed to access the resource.
        /// </summary>
        public IList<string> Origins { get; } = new List<string>();

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
        /// Gets or sets a value indicating whether the resource supports user credentials in the request.
        /// </summary>
        public bool SupportsCredentials { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("AllowAnyHeader: ");
            builder.Append(AllowAnyHeader);
            builder.Append(", AllowAnyMethod: ");
            builder.Append(AllowAnyMethod);
            builder.Append(", AllowAnyOrigin: ");
            builder.Append(AllowAnyOrigin);
            builder.Append(", PreflightMaxAge: ");
            builder.Append(PreflightMaxAge.HasValue ? 
                PreflightMaxAge.Value.ToString(CultureInfo.InvariantCulture) : "null");
            builder.Append(", SupportsCredentials: ");
            builder.Append(SupportsCredentials);
            builder.Append(", Origins: {");
            builder.Append(string.Join(",", Origins));
            builder.Append("}");
            builder.Append(", Methods: {");
            builder.Append(string.Join(",", Methods));
            builder.Append("}");
            builder.Append(", Headers: {");
            builder.Append(string.Join(",", Headers));
            builder.Append("}");
            builder.Append(", ExposedHeaders: {");
            builder.Append(string.Join(",", ExposedHeaders));
            builder.Append("}");
            return builder.ToString();
        }
    }
}