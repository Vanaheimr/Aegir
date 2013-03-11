/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Reflection;

using eu.Vanaheimr.Hermod.HTTP;

#endregion

namespace eu.Vanaheimr.Aegir.HTTPServer
{

    /// <summary>
    /// Any content representation.
    /// </summary>
    public class AegirHTTPService_ALL : AAegirHTTPService
    {

        #region Constructor(s)

        #region AegirHTTPService_ALL()

        /// <summary>
        /// Creates a new Aegir HTTP service.
        /// </summary>
        public AegirHTTPService_ALL()
            : base(HTTPContentType.ALL)
        { }

        #endregion

        #region AegirHTTPService_ALL(IHTTPConnection)

        /// <summary>
        /// Creates a new Aegir HTTP service.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public AegirHTTPService_ALL(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.ALL)
        {
            this.CallingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #endregion

    }

}
