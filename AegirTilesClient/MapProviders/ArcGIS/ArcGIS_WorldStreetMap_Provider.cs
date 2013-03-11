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

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// An ArcGIS WorldStreetMap provider
    /// </summary>
    public class ArcGIS_WorldStreetMap_Provider : AMapProvider
    {

        #region Data

        /// <summary>
        /// The well-known name for this map provider.
        /// </summary>
        public static String Name = "ArcGIS_WorldStreetMap";

        #endregion

        #region Constructor(s)

        #region ArcGIS_WorldStreetMap_Provider()

        /// <summary>
        /// Creates a new ArcGIS WorldStreetMap provider.
        /// </summary>
        public ArcGIS_WorldStreetMap_Provider()
            : base(Name:               Name,
                   Description:        "ArcGIS WorldStreetMap",
                   InfoUri:            "http://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer",
                   Copyright:          "",
                   IsMemoryCacheable:  true,
                   MemoryCacheEnabled: true,
                   UriPattern:         "/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{zoom}/{y}/{x}",
                   Hosts:              new String[1] { "http://server.arcgisonline.com" })
        { }

        #endregion

        #endregion

    }

}
