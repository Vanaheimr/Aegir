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
    /// HTML content representation.
    /// </summary>
    public class AegirHTTPService_HTML : AAegirHTTPService
    {

        #region Constructor(s)

        #region AegirHTTPService_HTML()

        /// <summary>
        /// Creates a new Aegir HTTP service.
        /// </summary>
        public AegirHTTPService_HTML()
            : base(HTTPContentType.HTML_UTF8)
        { }

        #endregion

        #region AegirHTTPService_HTML(IHTTPConnection)

        /// <summary>
        /// Creates a new Aegir HTTP service.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public AegirHTTPService_HTML(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.HTML_UTF8)
        {
            this.CallingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #endregion


        #region (private) HTMLBuilder(Headline, StringBuilderFunc)

        private String HTMLBuilder(String Headline, Action<StringBuilder> StringBuilderFunc)
        {

            var _StringBuilder = new StringBuilder();

            _StringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            _StringBuilder.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            _StringBuilder.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            _StringBuilder.AppendLine("<head>");
            _StringBuilder.AppendLine("<title>Hermod HTTP Server</title>");
            _StringBuilder.AppendLine("</head>");
            _StringBuilder.AppendLine("<body>");
            _StringBuilder.Append("<h2>").Append(Headline).AppendLine("</h2>");
            _StringBuilder.AppendLine("<table>");
            _StringBuilder.AppendLine("<tr>");
            _StringBuilder.AppendLine("<td style=\"width: 100px\">&nbsp;</td>");
            _StringBuilder.AppendLine("<td>");

            StringBuilderFunc(_StringBuilder);

            _StringBuilder.AppendLine("</td>");
            _StringBuilder.AppendLine("</tr>");
            _StringBuilder.AppendLine("</table>");
            _StringBuilder.AppendLine("</body>").AppendLine("</html>").AppendLine();

            return _StringBuilder.ToString();

        }

        #endregion


        #region /graphs

        public override HTTPResponse GET_Graphs()
        {

            var AllGraphs = GraphServer.Select(graph => "<a href=\"/graph/" + graph.Id + "\">" + graph.Id + " - " + graph.Description + "</a> " +
                                                        "<a href=\"/graph/" + graph.Id + "/vertices\">[All Vertices]</a> " +
                                                        "<a href=\"/graph/" + graph.Id + "/edges\">[All Edge]</a>").
                                        Aggregate((a, b) => a + "<br>" + b);

            return new HTTPResponseBuilder()
            {
                HTTPStatusCode = HTTPStatusCode.OK,
                ContentType    = HTTPContentType.HTML_UTF8,
                Content        = HTMLBuilder("", StringBuilder => StringBuilder.AppendLine(AllGraphs)).ToUTF8Bytes()
            };

        }

        #endregion

        #region Vertices(GraphId)

        /// <summary>
        /// Return all vertices of the given graph.
        /// </summary>
        /// <param name="GraphId">The identification of the graph.</param>
        public override HTTPResponse GET_Vertices(String GraphId)
        {
            
            var StringBuilder  = new StringBuilder();
            var VerticesResult = base.GET_Vertices_protected(GraphId);

            if (VerticesResult.HasErrors)
                return VerticesResult.Error;

            if (VerticesResult.Data.Any())
            {

                StringBuilder.Append("<table>");

                VerticesResult.Data.ForEach(Vertex =>
                    {

                        StringBuilder.Append("<tr><td>");
                        StringBuilder.Append("<table>");

                        Vertex.ForEach(KeyValuePair =>
                            StringBuilder.Append("<tr><td>").
                                          Append(KeyValuePair.Key.ToString()).
                                          Append("</td><td>").
                                          Append(KeyValuePair.Value.ToString()).
                                          Append("</td></tr>"));

                        StringBuilder.Append("</table>");
                        StringBuilder.AppendLine("</td></tr>");

                    });

                StringBuilder.Append("</table>");

            }

            return new HTTPResponseBuilder() {
                HTTPStatusCode = HTTPStatusCode.OK,
                ContentType    = this.HTTPContentTypes.First(),
                Content        = HTMLBuilder("All vertices", sb => sb.Append(StringBuilder.ToString())).ToUTF8Bytes()
            };

        }

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
                
                StringBuilder.Append("<table>");
                
                StringBuilder.Append("<tr>").
                              Append("<td>Id</td>").
                              Append("<td>Description</td>").
                              Append("<td>Latitude</td>").
                              Append("<td>Longitude</td>").
                              Append("</tr>");

                Result.Data.ForEach(Vertex => StringBuilder.Append("<tr>").
                                                            Append("<td>").Append(Vertex.Id.ToString()).Append("</td>").
                                                            Append("<td>").Append(Vertex.Description).Append("</td>").
                                                            Append("<td>").Append(Vertex.GetDouble(Semantics.Latitude).ToString()).Append("</td>").
                                                            Append("<td>").Append(Vertex.GetDouble(Semantics.Longitude).ToString()).Append("</td>").
                                                            Append("</tr>"));
                
                StringBuilder.Append("</table>");

            }

            return new HTTPResponseBuilder() {
                HTTPStatusCode = HTTPStatusCode.OK,
                ContentType    = this.HTTPContentTypes.First(),
                Content        = HTMLBuilder("All entries", sb => sb.Append(StringBuilder.ToString())).ToUTF8Bytes()
            };

        }

        #endregion

    }

}
