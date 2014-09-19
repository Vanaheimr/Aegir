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
    /// An Open Street Map Provider
    /// </summary>
    public class OSMProvider : MapTilesProvider
    {

        #region Data

        /// <summary>
        /// The well-known name for this map provider.
        /// </summary>
        public static String Name = "OSM";

        #endregion

        #region Constructor(s)

        #region OSMProvider()

        /// <summary>
        /// Creates a new OpenStreetMap map provider.
        /// </summary>
        public OSMProvider()
            : base(Id:                 Name,
                   Description:        "OpenStreetMap maps",
                   InfoUri:            "",
                   Copyright:          "CC",
                   IsMemoryCacheable:  true,
                   MemoryCacheEnabled: true,
                   ZoomRange:          new Range<Byte>(0, 23),
                   UriPatterns:        new String[1] { "http://tile.openstreetmap.org/{zoom}/{x}/{y}.png" })
        { }

        #endregion

        #endregion

    }

}
