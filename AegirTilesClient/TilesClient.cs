﻿/*
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
using System.Linq;
using System.Collections.Generic;

using de.ahzf.Illias.Commons;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// An Aegir tile client.
    /// </summary>
    public class TileClient : ITileClient
    {

        #region Data

        private readonly AutoDiscovery<IMapProvider> AutoMapProviders;

        #endregion
        
        #region Properties

        #region RegisteredMapProviderNames

        /// <summary>
        /// Return an enumeration of all registered map provider names.
        /// </summary>
        public IEnumerable<String> RegisteredMapProviderNames
        {
            get
            {
                return AutoMapProviders.RegisteredNames;
            }
        }

        #endregion

        #region RegisteredMapProviders

        /// <summary>
        /// Return an enumeration of all registered map providers.
        /// </summary>
        public IDictionary<String, IMapProvider> RegisteredMapProviders
        {
            get
            {
                return AutoMapProviders.RegisteredTypes.ToDictionary(MapProvider => MapProvider.Name,
                                                                     MapProvider => MapProvider);
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region TileClient()

        /// <summary>
        /// Create a new tile client.
        /// </summary>
        public TileClient()
        {
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true, Mapprovider => Mapprovider.Name);
        }

        #endregion

        #endregion


        #region GetTile(MapProviderName, Zoom, X, Y)

        /// <summary>
        /// Return the tile for the given parameters.
        /// </summary>
        /// <param name="MapProviderName">The unique identification of the map provider.</param>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <param name="X">The x coordinate of the tile.</param>
        /// <param name="Y">The y coordinate of the tile.</param>
        /// <returns>The requested tile as an array of bytes.</returns>
        public Byte[] GetTile(String MapProviderName, UInt32 ZoomLevel, UInt32 X, UInt32 Y)
        {
            
            IMapProvider _MapProvider = null;
            if (AutoMapProviders.TryGetInstance(MapProviderName, out _MapProvider))
            {
                return _MapProvider.GetTile(ZoomLevel, X, Y);
            }

            return null;

        }

        #endregion


    }

}