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

using de.ahzf.Hermod.HTTP;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// The Gera service interface mapping HTTP/REST URIs onto .NET methods.
    /// </summary>
    //[HTTPService(Host: "localhost:8080", ForceAuthentication: true)]
    [HTTPService]
    public interface ITileService : IHTTPService
    {

        TileServer TileServer { get; set; }

        #region ListMapProviders()

        /// <summary>
        /// Get an enumeration of all registered map providers.
        /// </summary>
        [NoAuthentication]
        [HTTPMapping(HTTPMethods.GET, "/MapProviders")]
        HTTPResponse ListMapProviders();

        #endregion

        #region ShowMapProviderInformation()

        /// <summary>
        /// Get detailed information on a map provider.
        /// </summary>
        [NoAuthentication]
        [HTTPMapping(HTTPMethods.GET, "/MapProvider/{Provider}")]
        HTTPResponse ShowMapProviderInformation(String Provider);

        #endregion

        #region GetTiles(Mapprovider, Zoom, X, Y)

        /// <summary>
        /// Get a tile from the given mapprovider at the
        /// given zoom level an coordinates.
        /// </summary>
        [NoAuthentication]
        [HTTPMapping(HTTPMethods.GET, "/tiles/{MapProvider}/{Zoom}/{X}/{Y}")]
        HTTPResponse GetTiles(String MapProvider, String Zoom, String X, String Y);

        #endregion

    }

}
