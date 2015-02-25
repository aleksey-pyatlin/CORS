// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNet.Cors.Core;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Cors
{
    /// <summary>
    /// Exposes methods to build a policy.
    /// </summary>
    public class CorsPolicyBuilder
    {
        private readonly CorsPolicy _policy = new CorsPolicy();

        /// <summary>
        /// Creates a new instance of the <see cref="CorsPolicyBuilder"/>.
        /// </summary>
        /// <param name="origins">list of origins which can be added.</param>
        public CorsPolicyBuilder(params string[] origins)
        {
        }

        public CorsPolicyBuilder(CorsPolicy policy)
        {
            Combine(policy);
        }

        public CorsPolicyBuilder Combine([NotNull] CorsPolicy policy)
        {
            AddOrigins(policy.Origins.ToArray());
            AddHeaders(policy.Headers.ToArray());
            AddExposedHeaders(policy.ExposedHeaders.ToArray());
            AddMethods(policy.Methods.ToArray());
            return this;
        }

        public CorsPolicyBuilder AddOrigins(params string[] origins)
        {
            foreach (var req in origins)
            {
                _policy.Origins.Add(req);
            }

            return this;
        }

        public CorsPolicyBuilder AddHeaders(params string[] headers)
        {
            foreach (var req in headers)
            {
                _policy.Headers.Add(req);
            }
            return this;
        }

        public CorsPolicyBuilder AddExposedHeaders(params string[] headers)
        {
            foreach (var req in headers)
            {
                _policy.ExposedHeaders.Add(req);
            }

            return this;
        }

        public CorsPolicyBuilder AddMethods(params string[] methods)
        {
            foreach (var req in methods)
            {
                _policy.Methods.Add(req);
            }

            return this;
        }

        public CorsPolicyBuilder AllowCredentials()
        {
            _policy.SupportsCredentials = true;
            return this;
        }

        public CorsPolicyBuilder AllowAnyOrigin()
        {
            _policy.Origins.Clear();
            _policy.Origins.Add(CorsConstants.AnyOrigin);
            return this;
        }

        public CorsPolicyBuilder AllowAnyMehtod()
        {
            _policy.AllowAnyMethod = true;
            return this;
        }

        public CorsPolicyBuilder AllowAnyHeader()
        {
            _policy.AllowAnyHeader = true;
            return this;
        }

        public CorsPolicy Build()
        {
            return _policy;
        }
    }
}