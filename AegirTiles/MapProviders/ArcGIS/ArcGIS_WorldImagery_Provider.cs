/*
 * Copyright (c) 2010-2012 Achim 'ahzf' Friedland <achim.friedland@aperis.com>
 * This file is part of TileServer <http://www.github.com/ahzf/TileServer>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

namespace de.ahzf.TileServer
{

    /// <summary>
    /// An ArcGIS WorldImagery provider
    /// </summary>
    public class ArcGIS_WorldImagery_Provider : AMapProvider
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
            : base(Name:               Name,
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
