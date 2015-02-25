// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Default implementation of <see cref="ICorsEngine"/>.
    /// </summary>
    public class CorsEngine : ICorsEngine
    {
        private readonly CorsOptions _options;

        /// <summary>
        /// Creates a new instance of the <see cref="CorsEngine"/>.
        /// </summary>
        /// <param name="options">The option model representing <see cref="CorsOptions"/>.</param>
        public CorsEngine([NotNull] IOptions<CorsOptions> options)
        {
            _options = options.Options;
        }

        /// <summary>
        /// Looks up a policy using the <paramref name="policyName"/> and then evaluates the policy using the passed in
        /// <paramref name="context"/>.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="policyName"></param>
        /// <returns>A <see cref="CorsResult"/> which contains the result of policy evaluation and can be
        /// used by the caller to set apporpriate response headers.</returns>
        public CorsResult EvaluatePolicy([NotNull] HttpContext context, string policyName)
        {
            var policy = _options.GetPolicy(policyName);
            return EvaluatePolicy(context, policy);
        }

        /// <inheritdoc />
        public CorsResult EvaluatePolicy([NotNull] HttpContext context, [NotNull]CorsPolicy policy)
        {
            var corsResult = new CorsResult();

            if (!TryValidateOrigin(context, policy, corsResult))
            {
                return corsResult;
            }

            corsResult.SupportsCredentials = policy.SupportsCredentials;
            if (context.Request.IsCorsRequest())
            {
                if (!TryValidateMethod(context, policy, corsResult))
                {
                    return corsResult;
                }
                if (!TryValidateHeaders(context, policy, corsResult))
                {
                    return corsResult;
                }

                corsResult.PreflightMaxAge = policy.PreflightMaxAge;
            }
            else
            {
                AddHeaderValues(corsResult.AllowedExposedHeaders, policy.ExposedHeaders);
            }

            return corsResult;
        }

        /// <summary>
        /// Try to validate the request origin based on <see cref="CorsPolicy"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="policy">The <see cref="CorsPolicy"/>.</param>
        /// <param name="result">The <see cref="CorsResult"/>.</param>
        /// <returns><c>true</c> if the request origin is valid; otherwise, <c>false</c>.</returns>
        public virtual bool TryValidateOrigin(
            [NotNull] HttpContext context,
            [NotNull] CorsPolicy policy,
            [NotNull] CorsResult result)
        {
            var origin = context.Request.Headers.Get(CorsConstants.Origin);
            if (origin != null)
            {
                if (policy.AllowAnyOrigin)
                {
                    if (policy.SupportsCredentials)
                    {
                        result.AllowedOrigin = origin;
                    }
                    else
                    {
                        result.AllowedOrigin = CorsConstants.AnyOrigin;
                    }
                }
                else
                {
                    if (policy.Origins.Contains(origin))
                    {
                        result.AllowedOrigin = origin;
                    }
                    else
                    {
                        result.ErrorMessages.Add(Resources.FormatOriginNotAllowed(origin));
                    }
                }
            }
            else
            {
                result.ErrorMessages.Add(Resources.OriginNotAllowed);
            }

            return result.IsValid;
        }

        /// <summary>
        /// Try to validate the requested method based on <see cref="CorsPolicy"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="policy">The <see cref="CorsPolicy"/>.</param>
        /// <param name="result">The <see cref="CorsResult"/>.</param>
        /// <returns><c>true</c> if the requested method is valid; otherwise, <c>false</c>.</returns>
        public virtual bool TryValidateMethod(
            [NotNull] HttpContext context,
            [NotNull] CorsPolicy policy,
            [NotNull] CorsResult result)
        {
            var accessControlRequestMethod = context.Request.Headers.Get(CorsConstants.AccessControlRequestMethod);
            if (policy.AllowAnyMethod || policy.Methods.Contains(accessControlRequestMethod))
            {
                result.AllowedMethods.Add(accessControlRequestMethod);
            }
            else
            {
                result.ErrorMessages.Add(Resources.MethodNotAllowed);
            }

            return result.IsValid;
        }

        /// <summary>
        /// Try to validate the requested headers based on <see cref="CorsPolicy"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="policy">The <see cref="CorsPolicy"/>.</param>
        /// <param name="result">The <see cref="CorsResult"/>.</param>
        /// <returns><c>true</c> if the requested headers are valid; otherwise, <c>false</c>.</returns>
        public virtual bool TryValidateHeaders(
            [NotNull] HttpContext context,
            [NotNull] CorsPolicy policy,
            [NotNull] CorsResult result)
        {
            var requestHeaders =
                context.Request.Headers.GetCommaSeparatedValues(CorsConstants.AccessControlRequestHeaders);
            if (policy.AllowAnyHeader || requestHeaders.All(header => policy.Headers.Contains(header)))
            {
                AddHeaderValues(result.AllowedHeaders, requestHeaders);
            }
            else
            {
                result.ErrorMessages.Add(Resources.HeadersNotAllowed);
            }

            return result.IsValid;
        }

        private static void AddHeaderValues(IList<string> target, IEnumerable<string> headerValues)
        {
            foreach (string current in headerValues)
            {
                target.Add(current);
            }
        }
    }
}