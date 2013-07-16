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
using System.IO;
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    #region ITileClientExtentions

    /// <summary>
    /// Extention methods for the ITileClient interface.
    /// </summary>
    public static class ITileClientExtentions
    {

        #region GetTileStream(MapProviderId, Zoom, X, Y)

        /// <summary>
        /// Return the tile for the given parameters.
        /// </summary>
        /// <param name="ITileClient">A ITileClient.</param>
        /// <param name="MapProviderId">The unique identification of the map provider.</param>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <param name="X">The x coordinate of the tile.</param>
        /// <param name="Y">The y coordinate of the tile.</param>
        /// <returns>The requested tile as a byte stream.</returns>
        public static Stream GetTileStream(this IAegirTilesClient ITileClient, String MapProviderId, UInt32 ZoomLevel, UInt32 X, UInt32 Y)
        {
            return new MemoryStream(ITileClient.GetTile(MapProviderId, ZoomLevel, X, Y));
        }

        #endregion

    }

    #endregion


    /// <summary>
    /// The common interface for all Aegir tile clients.
    /// </summary>
    public interface IAegirTilesClient
    {

        /// <summary>
        /// Return an enumeration of all map provider names.
        /// </summary>
        IEnumerable<String> RegisteredMapProviderIds { get; }

        /// <summary>
        /// Return an enumeration of all map providers.
        /// </summary>
        IDictionary<String, IMapProvider> RegisteredMapProviders { get; }


        /// <summary>
        /// Return the tile for the given parameters.
        /// </summary>
        /// <param name="MapProviderId">The unique identification of the map provider.</param>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <param name="X">The x coordinate of the tile.</param>
        /// <param name="Y">The y coordinate of the tile.</param>
        /// <returns>The requested tile as an array of bytes.</returns>
        Byte[] GetTile(String MapProviderId, UInt32 ZoomLevel, UInt32 X, UInt32 Y);

    }

}
