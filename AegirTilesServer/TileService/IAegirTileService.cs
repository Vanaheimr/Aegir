/*
 * Copyright (c) 2011-2013 Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
 * 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 * 
 * You may obtain a copy of the License at
 *   http://www.gnu.org/licenses/gpl.html
 * 
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 */

#region Usings

using System;

using eu.Vanaheimr.Hermod.HTTP;

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// The interface for the Aegir tile service.
    /// </summary>
    //[HTTPService(Host: "localhost:8080", ForceAuthentication: true)]
    [HTTPService]
    public interface IAegirTilesService : IHTTPService
    {

        #region TileServer

        /// <summary>
        /// The associated Aegir tiles server.
        /// </summary>
        AegirTilesServer TilesServer { get; set; }

        #endregion


        #region GET_MapProviders()

        /// <summary>
        /// Get an enumeration of all registered map providers.
        /// </summary>
        [NoAuthentication]
        [HTTPMapping(HTTPMethods.GET, "/MapProviders")]
        HTTPResponse GET_MapProviders();

        #endregion

        #region GET_MapProvider(MapProviderId)

        /// <summary>
        /// Get detailed information on a map provider.
        /// </summary>
        [NoAuthentication]
        [HTTPMapping(HTTPMethods.GET, "/MapProvider/{MapProviderId}")]
        HTTPResponse GET_MapProvider(String MapProviderId);

        #endregion

        #region GET_Tile(MapProviderId, Zoom, X, Y)

        /// <summary>
        /// Get a tile from the given map provider at the
        /// given zoom level and coordinates.
        /// </summary>
        [NoAuthentication]
        [HTTPMapping(HTTPMethods.GET, "/MapProvider/{MapProviderId}/tiles/{Zoom}/{X}/{Y}")]
        HTTPResponse GET_Tile(String MapProviderId, String Zoom, String X, String Y);

        #endregion

    }

}
