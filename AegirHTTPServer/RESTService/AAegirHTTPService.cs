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
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using de.ahzf.Illias.Commons.Collections;
using de.ahzf.Hermod.HTTP;
using de.ahzf.Vanaheimr.Bifrost.HTTP.Server;
using de.ahzf.Vanaheimr.Aegir.Tiles;
using de.ahzf.Vanaheimr.Blueprints;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.HTTPServer
{

    /// <summary>
    /// This class provides the generic ILinkedEnergyService functionality
    /// without being bound to any specific content representation.
    /// </summary>
    public abstract class AAegirHTTPService : ABifrostService,
                                              IAegirHTTPService

    {

        #region Constructor(s)

        #region AAegirHTTPService()

        /// <summary>
        /// Creates a new abstract Aegir HTTP service.
        /// </summary>
        public AAegirHTTPService()
        { }

        #endregion

        #region AAegirHTTPService(HTTPContentType)

        /// <summary>
        /// Creates a new abstract Aegir HTTP service.
        /// </summary>
        /// <param name="HTTPContentType">A content type.</param>
        public AAegirHTTPService(HTTPContentType HTTPContentType)
            : base(HTTPContentType)
        { }

        #endregion

        #region AAegirHTTPService(HTTPContentTypes)

        /// <summary>
        /// Creates a new abstract Aegir HTTP service.
        /// </summary>
        /// <param name="HTTPContentTypes">A content type.</param>
        public AAegirHTTPService(IEnumerable<HTTPContentType> HTTPContentTypes)
            : base(HTTPContentTypes)
        { }

        #endregion

        #region AAegirHTTPService(IHTTPConnection, HTTPContentType)

        /// <summary>
        /// Creates a new abstract Aegir HTTP service.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        /// <param name="HTTPContentType">A content type.</param>
        /// <param name="ResourcePath">The path to internal resources.</param>
        public AAegirHTTPService(IHTTPConnection IHTTPConnection, HTTPContentType HTTPContentType)
            : base(IHTTPConnection, HTTPContentType, "LinkedEnergyMap.resources.")
        { }

        #endregion

        #region AAegirHTTPService(IHTTPConnection, HTTPContentTypes)

        /// <summary>
        /// Creates a new abstract Aegir HTTP service.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        /// <param name="HTTPContentTypes">An enumeration of content types.</param>
        /// <param name="ResourcePath">The path to internal resources.</param>
        public AAegirHTTPService(IHTTPConnection IHTTPConnection, IEnumerable<HTTPContentType> HTTPContentTypes)
            : base(IHTTPConnection, HTTPContentTypes, "LinkedEnergyMap.resources.")
        { }

        #endregion

        #endregion


        #region IAegirHTTPService Members

        #region TileServer

        /// <summary>
        /// The associated tile server.
        /// </summary>
        public IAegirTilesServer TilesServer { get; set; }

        #endregion


        #region GET_MapProviders()

        /// <summary>
        /// Get an enumeration of all registered map providers.
        /// </summary>
        public virtual HTTPResponse GET_MapProviders()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GET_MapProvider(MapProviderName)

        /// <summary>
        /// Get detailed information on a map provider.
        /// </summary>
        public virtual HTTPResponse GET_MapProvider(String MapProviderName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GET_Tile(MapProviderName, Zoom, X, Y)

        /// <summary>
        /// Get a tile from the given map provider at the
        /// given zoom level and coordinates.
        /// </summary>
        public virtual HTTPResponse GET_Tile(String MapProviderName, String Zoom, String X, String Y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion


        #region (protected) GET_VerticesInArea(GraphId, Latitude1, Longitude1, Latitude2, Longitude2)

        /// <summary>
        /// Get all vertices within the given area.
        /// </summary>
        /// <param name="GraphId">The unique identification of the graph.</param>
        /// <param name="Latitude1">The first latitude.</param>
        /// <param name="Longitude1">The first longitude.</param>
        /// <param name="Latitude2">The second latitude.</param>
        /// <param name="Longitude2">The second longitude.</param>
        public virtual HTTPResponse GET_VerticesInArea(String GraphId, String Latitude1, String Longitude1, String Latitude2, String Longitude2)
        {
            return HTTPErrors.HTTPErrorResponse(IHTTPConnection.InHTTPRequest, HTTPStatusCode.NotAcceptable);
        }

        /// <summary>
        /// Get all vertices within the given area.
        /// </summary>
        /// <param name="GraphId">The unique identification of the graph.</param>
        /// <param name="Latitude1">The first latitude.</param>
        /// <param name="Longitude1">The first longitude.</param>
        /// <param name="Latitude2">The second latitude.</param>
        /// <param name="Longitude2">The second longitude.</param>
        protected HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                String, Int64, String, String, Object,
                                                                String, Int64, String, String, Object,
                                                                String, Int64, String, String, Object>>>

            GET_VerticesInArea_protected(String GraphId, String Latitude1, String Longitude1, String Latitude2, String Longitude2)

        {

            ParseGraphId(GraphId);
            ParseSkipParameter();
            ParseTakeParameter();

            Double _Latitude1, _Longitude1, _Latitude2, _Longitude2;

            #region Parse and check Latitude1

            if (!GeoCoordinate.IsDecimalRegExpr.IsMatch(Latitude1) || !Double.TryParse(Latitude1.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out _Latitude1))
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The first latitude could not be parsed!");

            }

            if (_Latitude1 < -90 || _Latitude1 > 90)
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The first latitude is invalid!");

            }

            #endregion

            #region Parse and check Longitude1

            if (!GeoCoordinate.IsDecimalRegExpr.IsMatch(Longitude1) || !Double.TryParse(Longitude1.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out _Longitude1))
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The first longitude could not be parsed!");

            }

            if (_Longitude1 < -180 || _Longitude1 > 180)
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The first longitude is invalid!");

            }

            #endregion

            #region Parse and check Latitude2

            if (!GeoCoordinate.IsDecimalRegExpr.IsMatch(Latitude2) || !Double.TryParse(Latitude2.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out _Latitude2))
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The second latitude could not be parsed!");

            }

            if (_Latitude2 < -90 || _Latitude2 > 90)
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The second latitude is invalid!");

            }

            #endregion

            #region Parse and check Longitude2

            var pos = Longitude2.IndexOf("/");
            if (pos > 0)
                Longitude2 = Longitude2.Substring(0, Longitude2.IndexOf("/"));

            if (!GeoCoordinate.IsDecimalRegExpr.IsMatch(Longitude2) || !Double.TryParse(Longitude2.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out _Longitude2))
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The second longitude could not be parsed!");

            }

            if (_Longitude2 < -180 || _Longitude2 > 180)
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The second longitude is invalid!");

            }

            #endregion

            return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                     String, Int64, String, String, Object,
                                                                     String, Int64, String, String, Object,
                                                                     String, Int64, String, String, Object>>>
                       (

                           from   Vertex
                           in     GraphServer.GetGraph("default").Vertices()
                           let    Latitude  = Vertex.GetDouble(Semantics.Latitude)
                           let    Longitude = Vertex.GetDouble(Semantics.Longitude)

                           //ToDo: Integrate QuadTree!
                           where  (Latitude  >= _Latitude1  && Latitude  <= _Latitude2 &&
                                   Longitude >= _Longitude1 && Longitude <= _Longitude2)

                           select Vertex

                       );

        }

        #endregion

        #region (protected) FILTER_VerticesInArea(GraphId, Latitude1, Longitude1, Latitude2, Longitude2)

        /// <summary>
        /// Get all vertices within the given area.
        /// </summary>
        /// <param name="GraphId">The unique identification of the graph.</param>
        /// <param name="Latitude1">The first latitude.</param>
        /// <param name="Longitude1">The first longitude.</param>
        /// <param name="Latitude2">The second latitude.</param>
        /// <param name="Longitude2">The second longitude.</param>
        public virtual HTTPResponse FILTER_VerticesInArea(String GraphId, String Latitude1, String Longitude1, String Latitude2, String Longitude2)
        {
            return HTTPErrors.HTTPErrorResponse(IHTTPConnection.InHTTPRequest, HTTPStatusCode.NotAcceptable);
        }

        /// <summary>
        /// Get all vertices within the given area.
        /// </summary>
        /// <param name="GraphId">The unique identification of the graph.</param>
        /// <param name="Latitude1">The first latitude.</param>
        /// <param name="Longitude1">The first longitude.</param>
        /// <param name="Latitude2">The second latitude.</param>
        /// <param name="Longitude2">The second longitude.</param>
        protected HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                String, Int64, String, String, Object,
                                                                String, Int64, String, String, Object,
                                                                String, Int64, String, String, Object>>>

            FILTER_MapElementsWithinBoundingBox_protected(String GraphId, String Latitude1, String Longitude1, String Latitude2, String Longitude2)

        {

            ParseGraphId(GraphId);
            ParseSkipParameter();
            ParseTakeParameter();

            Double _Latitude1, _Longitude1, _Latitude2, _Longitude2;

            #region Parse and check Latitude1

            if (!GeoCoordinate.IsDecimalRegExpr.IsMatch(Latitude1) || !Double.TryParse(Latitude1.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out _Latitude1))
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The first latitude could not be parsed!");

            }

            if (_Latitude1 < -90 || _Latitude1 > 90)
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The first latitude is invalid!");

            }

            #endregion

            #region Parse and check Longitude1

            if (!GeoCoordinate.IsDecimalRegExpr.IsMatch(Longitude1) || !Double.TryParse(Longitude1.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out _Longitude1))
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The first longitude could not be parsed!");

            }

            if (_Longitude1 < -180 || _Longitude1 > 180)
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The first longitude is invalid!");

            }

            #endregion

            #region Parse and check Latitude2

            if (!GeoCoordinate.IsDecimalRegExpr.IsMatch(Latitude2) || !Double.TryParse(Latitude2.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out _Latitude2))
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The second latitude could not be parsed!");

            }

            if (_Latitude2 < -90 || _Latitude2 > 90)
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The second latitude is invalid!");

            }

            #endregion

            #region Parse and check Longitude2

            var pos = Longitude2.IndexOf("/");
            if (pos > 0)
                Longitude2 = Longitude2.Substring(0, Longitude2.IndexOf("/"));

            if (!GeoCoordinate.IsDecimalRegExpr.IsMatch(Longitude2) || !Double.TryParse(Longitude2.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out _Longitude2))
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The second longitude could not be parsed!");

            }

            if (_Longitude2 < -180 || _Longitude2 > 180)
            {

                return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object,
                                                                         String, Int64, String, String, Object>>>
                       (IHTTPConnection.InHTTPRequest, HTTPStatusCode.BadRequest, "The second longitude is invalid!");

            }

            #endregion

            return new HTTPResult<IEnumerable<IGenericPropertyVertex<String, Int64, String, String, Object,
                                                                     String, Int64, String, String, Object,
                                                                     String, Int64, String, String, Object,
                                                                     String, Int64, String, String, Object>>>
                       (

                           from   Vertex
                           in     GraphServer.GetGraph("default").Vertices()
                           let    Latitude  = Vertex.GetDouble(Semantics.Latitude)
                           let    Longitude = Vertex.GetDouble(Semantics.Longitude)

                           //ToDo: Integrate QuadTree!
                           where  (Latitude  >= _Latitude1  && Latitude  <= _Latitude2 &&
                                   Longitude >= _Longitude1 && Longitude <= _Longitude2)

                           select Vertex

                       );

        }

        #endregion


        #region (protected) GET /graph/{GraphId}/near_by/{Latitude}/{Longitude}/{MaxDistance}

        /// <summary>
        /// Get all vertices near the given location.
        /// </summary>
        public virtual HTTPResponse GET_VerticesNearBy(String Latitude, String Longitude, String MaxDistance)
        {
            return HTTPErrors.HTTPErrorResponse(IHTTPConnection.InHTTPRequest, HTTPStatusCode.NotAcceptable);
        }

        #endregion

        #region (protected) FILTER /graph/{GraphId}/near_by/{Latitude}/{Longitude}/{MaxDistance}

        /// <summary>
        /// Filter all vertices near the given location.
        /// </summary>
        public virtual HTTPResponse FILTER_VerticesNearBy(String Latitude, String Longitude, String MaxDistance)
        {
            return HTTPErrors.HTTPErrorResponse(IHTTPConnection.InHTTPRequest, HTTPStatusCode.NotAcceptable);
        }

        #endregion


    }

}

