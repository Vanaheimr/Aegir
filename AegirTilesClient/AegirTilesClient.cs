﻿/*
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
using System.Linq;
using System.Collections.Generic;

using eu.Vanaheimr.Illias.Commons;

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// An Aegir tiles client.
    /// </summary>
    public class AegirTilesClient : IAegirTilesClient
    {

        #region Data

        private readonly AutoDiscovery<IMapProvider> AutoMapProviders;

        #endregion
        
        #region Properties

        #region RegisteredMapProviderIds

        /// <summary>
        /// Return an enumeration of all registered map provider identifications.
        /// </summary>
        public IEnumerable<String> RegisteredMapProviderIds
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
                return AutoMapProviders.RegisteredTypes.ToDictionary(MapProvider => MapProvider.Id,
                                                                     MapProvider => MapProvider);
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region AegirTilesClient()

        /// <summary>
        /// Create a new tile client.
        /// </summary>
        public AegirTilesClient()
        {
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true, Mapprovider => Mapprovider.Id);
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
