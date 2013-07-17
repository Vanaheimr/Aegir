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
using System.Linq;
using System.Text;
using System.Reflection;

using eu.Vanaheimr.Hermod.HTTP;

using Newtonsoft.Json.Linq;

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// JSON content representation.
    /// </summary>
    public class AegirTilesService_JSON : AAegirTileService
    {

        #region Data

        private readonly Byte[] JSON_Success;
        
        #endregion

        #region Constructor(s)

        #region AegirTilesService_JSON()

        /// <summary>
        /// Creates a new tile service for JSON content.
        /// </summary>
        public AegirTilesService_JSON()
            : base(HTTPContentType.JSON_UTF8)
        { }

        #endregion

        #region AegirTilesService_JSON(IHTTPConnection)

        /// <summary>
        /// Creates a new tile service for JSON content.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public AegirTilesService_JSON(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.JSON_UTF8)
        {
            this.CallingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #endregion


        /// <summary>
        /// Get a tile from the given mapprovider at the
        /// given zoom level an coordinates.
        /// </summary>
        public HTTPResponse GET_Tile(String Mapprovider, String Zoom, String X, String Y)
        {
            throw new NotImplementedException();
        }

        #region ListMapProviders()

        /// <summary>
        /// Get an enumeration of all registered map providers.
        /// </summary>
        /// <example>
        /// $ curl -X GET  -H "Accept: application/json" http://127.0.0.1:8182/MapProviders
        /// {
        ///   "MapProviders": [
        ///     "OSM"
        ///   ]
        /// }
        /// </example>
        public HTTPResponse ListMapProviders()
        {

            var _Content = Encoding.UTF8.GetBytes(new JObject(
                               new JProperty(__MapProviders, TilesServer.RegisteredMapProviderIds)
                           ).ToString());

            return new HTTPResponseBuilder()
                {
                    HTTPStatusCode = HTTPStatusCode.OK,
                    CacheControl   = "no-cache",
                    ContentType    = HTTPContentType.JSON_UTF8,
                    Content        = _Content
                };

        }

        #endregion

        #region ShowMapProviderInformation(Provider)

        /// <summary>
        /// Get detailed information on a map provider.
        /// </summary>
        /// <param name=Provider>A map provider name.</param>
        /// <example>
        /// $ curl -X GET -H "Accept: application/json" http://127.0.0.1:8182/MapProvider/OSM
        /// {
        ///   "Name": "OSM",
        ///   "Description": ""
        /// }
        /// </example>
        public HTTPResponse ShowMapProviderInformation(String Provider)
        {

            IMapTilesProvider _MapProvider;

            if (TilesServer.RegisteredMapProviders.TryGetValue(Provider, out _MapProvider))
            {

                var _Content = Encoding.UTF8.GetBytes(new JObject(
                                   new JProperty(__MapProviderId,        _MapProvider.Id),
                                   new JProperty(__MapProviderDescription, _MapProvider.Description),
                                   new JProperty(__MapProviderUriPattern,  _MapProvider.UriPattern),
                                   new JProperty(__MapProviderHosts,       new JArray(
                                                                               from   _Host
                                                                               in     _MapProvider.Hosts
                                                                               select _Host))
                               ).ToString());

                return new HTTPResponseBuilder()
                    {
                        HTTPStatusCode = HTTPStatusCode.OK,
                        CacheControl   = "no-cache",
                        ContentLength  = (UInt64) _Content.Length,
                        ContentType    = HTTPContentType.JSON_UTF8,
                        Content        = _Content
                    };

            }


            #region ...invalid MapProvider!

            else
            {
                return new HTTPResponseBuilder()
                    {
                        HTTPStatusCode = HTTPStatusCode.NotFound,
                        CacheControl   = "no-cache",
                        ContentLength  = 0,
                    };
            }

            #endregion

        }

        #endregion

    }

}

