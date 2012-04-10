/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <code@ahzf.de>
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
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using de.ahzf.Hermod.HTTP;

#endregion

namespace de.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// An abstract tile service implementation.
    /// </summary>
    public abstract class ATileService : AHTTPService, ITileService
    {

        #region Data

        protected const String __MapProvider            = "MapProvider";
        protected const String __MapProviders           = "MapProviders";

        protected const String __MapProviderName        = "Name";
        protected const String __MapProviderDescription = "Description";
        protected const String __MapProviderUriPattern  = "UriPattern";
        protected const String __MapProviderHosts       = "Hosts";

        #endregion

        #region Properties

        public TileServer TileServer { get; set; }

        #endregion

        #region Constructor(s)

        #region ATileService()

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        public ATileService()
        { }

        #endregion

        #region ATileService(HTTPContentType)

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        /// <param name="HTTPContentType">A content type.</param>
        public ATileService(HTTPContentType HTTPContentType)
            : base(HTTPContentType)
        { }

        #endregion

        #region ATileService(HTTPContentTypes)

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        /// <param name="HTTPContentTypes">A content type.</param>
        public ATileService(IEnumerable<HTTPContentType> HTTPContentTypes)
            : base(HTTPContentTypes)
        { }

        #endregion

        #region ATileService(IHTTPConnection, HTTPContentType)

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        /// <param name="HTTPContentType">A content type.</param>
        /// <param name="ResourcePath">The path to internal resources.</param>
        public ATileService(IHTTPConnection IHTTPConnection, HTTPContentType HTTPContentType)
            : base(IHTTPConnection, HTTPContentType, "TileServer.resources.")
        { }

        #endregion

        #region ATileService(IHTTPConnection, HTTPContentTypes)

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        /// <param name="HTTPContentTypes">An enumeration of content types.</param>
        /// <param name="ResourcePath">The path to internal resources.</param>
        public ATileService(IHTTPConnection IHTTPConnection, IEnumerable<HTTPContentType> HTTPContentTypes)
            : base(IHTTPConnection, HTTPContentTypes, "TileServer.resources.")
        { }

        #endregion

        #endregion


        #region GetRoot()

        public virtual HTTPResponse GetRoot()
        {
            //return GetResources("landingpage.html");
            return Error406_NotAcceptable();
        }

        #endregion


        public HTTPResponse GetLandingpage()
        {
            throw new NotImplementedException();
        }

        public HTTPResponse ListMapProviders()
        {
            throw new NotImplementedException();
        }

        public HTTPResponse ShowMapProviderInformation(string Provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a tile from the given mapprovider at the
        /// given zoom level an coordinates.
        /// </summary>
        public HTTPResponse GetTiles(String Mapprovider, String Zoom, String X, String Y)
        {
            throw new NotImplementedException();
        }

        public HTTPResponse GetError(String myHTTPStatusCode)
        {
            throw new NotImplementedException();
        }

    }

}

