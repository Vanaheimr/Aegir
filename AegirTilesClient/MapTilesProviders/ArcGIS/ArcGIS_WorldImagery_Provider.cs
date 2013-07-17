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

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// An ArcGIS WorldImagery provider
    /// </summary>
    public class ArcGIS_WorldImagery_Provider : MapTilesProvider
    {

        #region Data

        /// <summary>
        /// The well-known name for this map provider.
        /// </summary>
        public static String Name = "ArcGIS_WorldImagery";

        #endregion

        #region Constructor(s)

        #region ArcGIS_WorldImagery_Provider()

        /// <summary>
        /// Creates a new ArcGIS WorldImagery provider.
        /// </summary>
        public ArcGIS_WorldImagery_Provider()
            : base(Id:               Name,
                   Description:        "ArcGIS WorldImagery",
                   InfoUri:            "http://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer",
                   Copyright:          "",
                   IsMemoryCacheable:  true,
                   MemoryCacheEnabled: true,
                   UriPattern:         "/ArcGIS/rest/services/World_Imagery/MapServer/tile/{zoom}/{y}/{x}",
                   Hosts:              new String[1] { "http://server.arcgisonline.com" })
        { }

        #endregion

        #endregion

    }

}
