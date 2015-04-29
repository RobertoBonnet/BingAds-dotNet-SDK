﻿//=====================================================================================================================================================
// Bing Ads .NET SDK ver. 9.3
// 
// Copyright (c) Microsoft Corporation
// 
// All rights reserved. 
// 
// MS-PL License
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. 
//  If you do not accept the license, do not use the software.
// 
// 1. Definitions
// 
// The terms reproduce, reproduction, derivative works, and distribution have the same meaning here as under U.S. copyright law. 
//  A contribution is the original software, or any additions or changes to the software. 
//  A contributor is any person that distributes its contribution under this license. 
//  Licensed patents  are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// 
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
//  each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, 
//  prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// 
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
//  each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, 
//  sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// 
// (A) No Trademark License - This license does not grant you rights to use any contributors' name, logo, or trademarks.
// 
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
//  your patent license from such contributor to the software ends automatically.
// 
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, 
//  and attribution notices that are present in the software.
// 
// (D) If you distribute any portion of the software in source code form, 
//  you may do so only under this license by including a complete copy of this license with your distribution. 
//  If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
// 
// (E) The software is licensed *as-is.* You bear the risk of using it. The contributors give no express warranties, guarantees or conditions.
//  You may have additional consumer rights under your local laws which this license cannot change. 
//  To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, 
//  fitness for a particular purpose and non-infringement.
//=====================================================================================================================================================

using System;
using System.Net.Http.Headers;

// ReSharper disable once CheckNamespace
namespace Microsoft.BingAds.Internal
{
    /// <summary>
    /// The abstract base class for all OAuth authentication classes. You can use this class to dynamically instantiate a derived OAuth authentication class at run time.
    /// This class cannot be instantiated, and instead you should use either <see cref="OAuthDesktopMobileAuthCodeGrant"/>, <see cref="OAuthDesktopMobileImplicitGrant"/>, 
    /// or <see cref="OAuthWebAuthCodeGrant"/>, which extend this class.
    /// </summary>    
    /// <seealso cref="OAuthDesktopMobileImplicitGrant"/>
    /// <seealso cref="OAuthDesktopMobileAuthCodeGrant"/>
    /// <seealso cref="OAuthWebAuthCodeGrant"/>
    public abstract class OAuthAuthorization : Authentication
    {
        private readonly string _clientId;

        /// <summary>
        /// The client identifier corresponding to your registered application.         
        /// </summary>
        /// <remarks>
        /// For more information about using a client identifier for authentication, see the 
        /// Client Password Authentication section of the OAuth 2.0 spec at http://tools.ietf.org/html/draft-ietf-oauth-v2-15#section-3.1
        /// </remarks>
        public string ClientId
        {
            get { return _clientId; }
        }

        /// <summary>
        /// The URI to which the user of the app will be redirected after receiving user consent.
        /// </summary>
        public abstract Uri RedirectionUri { get; }

        /// <summary>
        /// Initializes a new instance of the OAuthAuthorization class with the specified <paramref name="clientId"/>.
        /// </summary>
        /// <param name="clientId">
        /// The client identifier corresponding to your registered application.         
        /// </param>
        /// <remarks>
        /// For more information about using a client identifier for authentication, see the 
        /// Client Password Authentication section of the OAuth 2.0 spec at http://tools.ietf.org/html/draft-ietf-oauth-v2-15#section-3.1
        /// </remarks>
        protected OAuthAuthorization(string clientId)
        {
            if (clientId == null)
            {
                throw new ArgumentNullException("clientId");
            }

            _clientId = clientId;
        }

        /// <summary>
        /// Sets the AuthenticationToken header element for the corresponding Bing Ads service operation. 
        /// </summary>
        /// <param name="apiRequest">The Bing Ads service operation request object.</param>
        /// <remarks>
        /// <para>
        /// This method will set the OAuth access token as the AuthenticationToken header on the API request object.
        /// </para>
        /// <para>
        /// For example, <paramref name="apiRequest"/> object can be the 
        /// GetUserRequest message of the GetUser service operation. For more information about GetUser, see http://go.microsoft.com/fwlink/?LinkID=511603. 
        /// </para>
        /// </remarks>
        protected internal override void SetAuthenticationFieldsOnApiRequestObject(dynamic apiRequest)
        {
            if (OAuthTokens == null)
            {
                throw new InvalidOperationException(ErrorMessages.GetFullOAuthAccessTokenNotRequestedMessage(GetType()));
            }

            apiRequest.AuthenticationToken = OAuthTokens.AccessToken;
        }

        /// <summary>
        /// Gets the Microsoft Account authorization endpoint where the user should be navigated to give his or her consent.
        /// </summary>
        /// <returns>The Microsoft Account authorization endpoint of type <see cref="Uri"/>.</returns>
        public abstract Uri GetAuthorizationEndpoint();

        /// <summary>
        /// Contains information about OAuth access tokens received from the Microsoft Account authorization service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// You can get OAuthTokens using the RequestAccessAndRefreshTokens method of one of the desktop or web application grant flow classes, 
        /// for example <see cref="OAuthWithAuthorizationCode.RequestAccessAndRefreshTokensAsync(System.Uri)"/>.
        /// </para>        
        /// </remarks>
        public OAuthTokens OAuthTokens { get; protected set; }

        /// <summary>
        /// Adds the AuthenticationToken header element for the corresponding bulk file upload operation. 
        /// </summary>
        /// <param name="requestHeaders">The headers collection to which authentication requests should be added.</param>
        protected internal override void AddAuthenticationHeaders(HttpRequestHeaders requestHeaders)
        {
            if (OAuthTokens == null)
            {
                throw new InvalidOperationException(ErrorMessages.GetFullOAuthAccessTokenNotRequestedMessage(GetType()));
            }

            requestHeaders.Add("AuthenticationToken", OAuthTokens.AccessToken);
        }
    }
}