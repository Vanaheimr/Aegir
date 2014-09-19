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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// An ArcGIS WorldStreetMap provider
    /// </summary>
    public class ArcGIS_WorldStreetMap_Provider : MapTilesProvider
    {

        #region Data

        /// <summary>
        /// The well-known name for this map provider.
        /// </summary>
        public static String Name = "ArcGIS_WorldStreetMap";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new ArcGIS WorldStreetMap provider.
        /// </summary>
        public ArcGIS_WorldStreetMap_Provider()
            : base(Id:                  Name,
                   Description:         "ArcGIS WorldStreetMap",
                   InfoUri:             "http://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer",
                   Copyright:           "",
                   IsMemoryCacheable:   true,
                   MemoryCacheEnabled:  true,
                   ZoomRange:           new Range<Byte>(0, 23),
                   UriPatterns:         new String[1] { "http://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{zoom}/{y}/{x}" })
        { }

        #endregion

    }

}
