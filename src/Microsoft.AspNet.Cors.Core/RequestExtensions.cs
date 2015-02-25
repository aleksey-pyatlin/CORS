// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Cors.Core
{
    /// <summary>
    /// Extension methods for <see cref="HttpRequest"/>.
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// Determines if the current <see cref="HttpRequest"/> is a pre-flight request.
        /// </summary>
        /// <param name="request">The given <see cref="HttpRequest"/>.</param>
        /// <returns><c>true</c> if there is an Origin header and there is a non null Access-Control-Request-Method
        /// header and the request method is OPTIONS.<c>false</c> otherwise.</returns>
        public static bool IsPreflight(this HttpRequest request)
        {
            var origin = request.Headers.Get(CorsConstants.Origin);
            var accessControlRequestMethod = request.Headers.Get(CorsConstants.AccessControlRequestMethod);
            return
                origin != null &&
                accessControlRequestMethod != null &&
                string.Equals(request.Method, CorsConstants.PreflightHttpMethod, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines if the current <see cref="HttpRequest"/> is a CORS request.
        /// </summary>
        /// <param name="request">The given <see cref="HttpRequest"/></param>
        /// <returns><c>true</c> if the request contains Origin header.</returns>
        public static bool IsCorsRequest(this HttpRequest request)
        {
            return request.Headers.ContainsKey(CorsConstants.Origin);
        }
    }
}