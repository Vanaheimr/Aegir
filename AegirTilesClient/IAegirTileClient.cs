/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <achim@graph-database.org>
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
using System.Collections.Generic;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.Tiles
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
