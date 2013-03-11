/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <code@ahzf.de>
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
using System.Text;
using System.Linq;
using System.Reflection;

using eu.Vanaheimr.Illias.Commons;
using eu.Vanaheimr.Illias.Commons.Collections;
using eu.Vanaheimr.Hermod.HTTP;
using eu.Vanaheimr.Bifrost.HTTP.Server;

#endregion

namespace eu.Vanaheimr.Aegir.HTTPServer
{

    /// <summary>
    /// JSON content representation.
    /// </summary>
    public class AegirHTTPService_JSON : AAegirHTTPService
    {

        #region Constructor(s)

        #region AegirHTTPService_JSON()

        /// <summary>
        /// Creates a new Linked Energy map service.
        /// </summary>
        public AegirHTTPService_JSON()
            : base(HTTPContentType.JSON_UTF8)
        { }

        #endregion

        #region AegirHTTPService_JSON(IHTTPConnection)

        /// <summary>
        /// Creates a new Linked Energy map service.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public AegirHTTPService_JSON(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.JSON_UTF8)
        {
            this.CallingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #endregion


        #region GET_MapElementsWithinBoundingBox_protected(GraphId, Latitude1, Longitude1, Latitude2, Longitude2)

        /// <summary>
        /// Get all elements within the given bounding box.
        /// </summary>
        /// <param name="GraphId">The unique identification of the graph.</param>
        /// <param name="Latitude1">The first latitude.</param>
        /// <param name="Longitude1">The first longitude.</param>
        /// <param name="Latitude2">The second latitude.</param>
        /// <param name="Longitude2">The second longitude.</param>
        public override HTTPResponse GET_VerticesInArea(String GraphId, String Latitude1, String Longitude1, String Latitude2, String Longitude2)
        {

            var StringBuilder  = new StringBuilder();
            var Result         = base.GET_VerticesInArea_protected(GraphId, Latitude1, Longitude1, Latitude2, Longitude2);

            if (Result.HasErrors)
                return Result.Error;

            if (Result.Data.Any())
            {
                
                StringBuilder.AppendLine("[");

                Result.Data.ForEach(Vertex => StringBuilder.Append("{ \"properties\": {").
                                                            Append("\"id\" : \"").         Append(Vertex.Id.ToString()).Append("\",").
                                                            Append("\"label\" : \"").      Append(Vertex.Label.ToString()).Append("\",").
                                                            Append("\"description\" : \"").Append(Vertex["Description"]).Append("\",").
                                                            Append("\"latitude\" : ").     Append(Vertex.GetDouble(Semantics.Latitude). ToString().Replace(",",".")).Append(",").
                                                            Append("\"longitude\" : ").    Append(Vertex.GetDouble(Semantics.Longitude).ToString().Replace(",",".")).
                                                            AppendLine("} },"));

                StringBuilder.Length = StringBuilder.Length - 3;

                StringBuilder.AppendLine();
                StringBuilder.AppendLine("]");

            }

            return new HTTPResponseBuilder() {
                HTTPStatusCode = HTTPStatusCode.OK,
                ContentType    = this.HTTPContentTypes.First(),
                Content        = StringBuilder.ToString().ToUTF8Bytes()
            };

        }

        #endregion


    }

}
