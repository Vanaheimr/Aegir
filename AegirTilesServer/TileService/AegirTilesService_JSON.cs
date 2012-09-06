/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <achim@graph-database.org>
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
using System.Text;
using System.Reflection;

using de.ahzf.Vanaheimr.Hermod.HTTP;

using Newtonsoft.Json.Linq;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.Tiles
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
            
            IMapProvider _MapProvider;

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

