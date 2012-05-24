/*
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

using de.ahzf.Hermod.HTTP;
using de.ahzf.Vanaheimr.Aegir.Tiles;
using de.ahzf.Vanaheimr.Bifrost.HTTP.Server;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.HTTPServer
{

    //[HTTPService(Host: "localhost:8080", ForceAuthentication: true)]
    [HTTPService(HostAuthentication: false)]
    public interface IAegirHTTPService : IBifrostService,
                                         IAegirTilesService

    {

        // Vertices on a map

        #region GET /graph/{GraphId}/vertices/in_area/{Latitude1}/{Longitude1}/{Latitude2}/{Longitude2}

        /// <summary>
        /// Get all vertices within the given area.
        /// </summary>
        /// <param name="GraphId">The unique identification of the graph.</param>
        /// <param name="Latitude1">The starting latitude.</param>
        /// <param name="Longitude1">The starting longitude.</param>
        /// <param name="Latitude2">The ending latitude.</param>
        /// <param name="Longitude2">The ending longitude.</param>
        /// <httpparam name="SKIP">Skip the given number of vertices from the beginning of the result set.</httpparam>
        /// <httpparam name="TAKE">Return only the given number of vertices from the result set.</httpparam>
        /// <httpparam name="SELECT"></httpparam>
        [HTTPMapping(HTTPMethods.GET, "/graph/{GraphId}/vertices/in_area/{Latitude1}/{Longitude1}/{Latitude2}/{Longitude2}"), NoAuthentication]
        HTTPResponse GET_VerticesInArea(String GraphId, String Latitude1, String Longitude1, String Latitude2, String Longitude2);

        #endregion

        #region FILTER /graph/{GraphId}/vertices/in_area/{Latitude1}/{Longitude1}/{Latitude2}/{Longitude2}

        /// <summary>
        /// Filter all vertices within the given area.
        /// </summary>
        /// <param name="GraphId">The unique identification of the graph.</param>
        /// <param name="Latitude1">The starting latitude.</param>
        /// <param name="Longitude1">The starting longitude.</param>
        /// <param name="Latitude2">The ending latitude.</param>
        /// <param name="Longitude2">The ending longitude.</param>
        [HTTPMapping(HTTPMethods.FILTER, "/graph/{GraphId}/vertices/in_area/{Latitude1}/{Longitude1}/{Latitude2}/{Longitude2}"), NoAuthentication]
        HTTPResponse FILTER_VerticesInArea(String GraphId, String Latitude1, String Longitude1, String Latitude2, String Longitude2);

        #endregion

        #region GET /graph/{GraphId}/vertices/near_by/{Latitude}/{Longitude}/{MaxDistance}

        /// <summary>
        /// Get all vertices near the given location.
        /// </summary>
        [HTTPMapping(HTTPMethods.GET, "/graph/{GraphId}/vertices/near_by/{Latitude}/{Longitude}/{MaxDistance}")]
        HTTPResponse GET_VerticesNearBy(String Latitude, String Longitude, String MaxDistance);

        #endregion

        #region FILTER /graph/{GraphId}/vertices/near_by/{Latitude}/{Longitude}/{MaxDistance}

        /// <summary>
        /// Filter all vertices near the given location.
        /// </summary>
        [HTTPMapping(HTTPMethods.FILTER, "/graph/{GraphId}/vertices/near_by/{Latitude}/{Longitude}/{MaxDistance}")]
        HTTPResponse FILTER_VerticesNearBy(String Latitude, String Longitude, String MaxDistance);

        #endregion


    }

}
