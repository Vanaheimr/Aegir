/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
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

using de.ahzf.Vanaheimr.Hermod.HTTP;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// An abstract Aegir tile service implementation.
    /// </summary>
    public abstract class AAegirTileService : AHTTPService,
                                              IAegirTilesService
    {

        #region Data

        protected const String __MapProvider            = "MapProvider";
        protected const String __MapProviders           = "MapProviders";

        protected const String __MapProviderId        = "Name";
        protected const String __MapProviderDescription = "Description";
        protected const String __MapProviderUriPattern  = "UriPattern";
        protected const String __MapProviderHosts       = "Hosts";

        #endregion

        #region Properties

        public IAegirTilesServer TilesServer { get; set; }

        #endregion

        #region Constructor(s)

        #region ATileService()

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        public AAegirTileService()
        { }

        #endregion

        #region ATileService(HTTPContentType)

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        /// <param name="HTTPContentType">A content type.</param>
        public AAegirTileService(HTTPContentType HTTPContentType)
            : base(HTTPContentType)
        { }

        #endregion

        #region ATileService(HTTPContentTypes)

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        /// <param name="HTTPContentTypes">A content type.</param>
        public AAegirTileService(IEnumerable<HTTPContentType> HTTPContentTypes)
            : base(HTTPContentTypes)
        { }

        #endregion

        #region ATileService(IHTTPConnection, HTTPContentType)

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        /// <param name="HTTPContentType">A content type.</param>
        public AAegirTileService(IHTTPConnection IHTTPConnection, HTTPContentType HTTPContentType)
            : base(IHTTPConnection, HTTPContentType, "TileServer.resources.")
        { }

        #endregion

        #region ATileService(IHTTPConnection, HTTPContentTypes)

        /// <summary>
        /// Creates a new abstract ATileService.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        /// <param name="HTTPContentTypes">An enumeration of content types.</param>
        public AAegirTileService(IHTTPConnection IHTTPConnection, IEnumerable<HTTPContentType> HTTPContentTypes)
            : base(IHTTPConnection, HTTPContentTypes, "TileServer.resources.")
        { }

        #endregion

        #endregion


        #region GET_Root()

        /// <summary>
        /// Get the landing page.
        /// </summary>
        public virtual HTTPResponse GET_Root()
        {
            return HTTPTools.MovedTemporarily("/MapProviders");
        }

        #endregion


        #region GET_MapProviders()

        /// <summary>
        /// Get an enumeration of all registered map providers.
        /// </summary>
        public virtual HTTPResponse GET_MapProviders()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GET_MapProvider(MapProviderId)

        /// <summary>
        /// Get detailed information on a map provider.
        /// </summary>
        public virtual HTTPResponse GET_MapProvider(String MapProviderId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GET_Tile(MapProviderId, Zoom, X, Y)

        /// <summary>
        /// Get a tile from the given map provider at the
        /// given zoom level and coordinates.
        /// </summary>
        public virtual HTTPResponse GET_Tile(String MapProviderId, String Zoom, String X, String Y)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

