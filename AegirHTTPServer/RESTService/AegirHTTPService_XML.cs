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

using de.ahzf.Illias.Commons;
using de.ahzf.Illias.Commons.Collections;
using de.ahzf.Vanaheimr.Blueprints;
using de.ahzf.Vanaheimr.Hermod.HTTP;
using de.ahzf.Vanaheimr.Bifrost.HTTP.Server;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.HTTPServer
{

    /// <summary>
    /// XML content representation.
    /// </summary>
    public class AegirHTTPService_XML : AAegirHTTPService
    {

        #region Constructor(s)

        #region AegirHTTPService_XML()

        /// <summary>
        /// Creates a new Aegir HTTP service.
        /// </summary>
        public AegirHTTPService_XML()
            : base(HTTPContentType.XML_UTF8)
        { }

        #endregion

        #region AegirHTTPService_XML(IHTTPConnection)

        /// <summary>
        /// Creates a new Aegir HTTP service.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public AegirHTTPService_XML(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.XML_UTF8)
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

                StringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                StringBuilder.AppendLine("<results>");

                Result.Data.ForEach(Vertex => StringBuilder.Append("<properties>").
                                                            Append("<id>").          Append(Vertex.Id.ToString()).Append("</id>").
                                                            Append("<label>").       Append(Vertex.Label.ToString()).Append("</label>").
                                                            Append("<description>"). Append(Vertex["Description"]).Append("</description>").
                                                            Append("<latitude>").    Append(Vertex.GetDouble(Semantics.Latitude). ToString().Replace(",",".")).Append("</latitude>").
                                                            Append("<longitude>").   Append(Vertex.GetDouble(Semantics.Longitude).ToString().Replace(",",".")).Append("</longitude>").
                                                            AppendLine("</properties>"));

                StringBuilder.AppendLine("</results>");
                StringBuilder.AppendLine("</xml>");

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
