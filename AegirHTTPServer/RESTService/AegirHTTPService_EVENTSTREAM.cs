﻿/*
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
using de.ahzf.Hermod.HTTP;
using de.ahzf.Bifrost.HTTP.Server;
using de.ahzf.Blueprints.PropertyGraphs;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.HTTPServer
{

    /// <summary>
    /// EVENTSTREAM content representation.
    /// </summary>
    public class AegirHTTPService_EVENTSTREAM : AAegirHTTPService
    {

        #region Constructor(s)

        #region AegirHTTPService_EVENTSTREAM()

        /// <summary>
        /// Creates a new Aegir HTTP service.
        /// </summary>
        public AegirHTTPService_EVENTSTREAM()
            : base(HTTPContentType.EVENTSTREAM)
        { }

        #endregion

        #region AegirHTTPService_EVENTSTREAM(IHTTPConnection)

        /// <summary>
        /// Creates a new Aegir HTTP service.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public AegirHTTPService_EVENTSTREAM(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.EVENTSTREAM)
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


        //#region GetRoot()

        //public override HTTPResponse GetRoot()
        //{

        //    var GraphDBs = GraphServer.AllGraphDBs().
        //                               Select(t => "<a href=\"" + t.Uri + "\">" + t.Name + "</a>").
        //                               Aggregate((a, b) =>  a + "<br>" + b);

        //    return new HTTPResponseBuilder() {
        //        HTTPStatusCode = HTTPStatusCode.OK,
        //        ContentType    = HTTPContentType.HTML_UTF8,
        //        Content        = HTMLBuilder("www.graph-database.org v0.1", b => b.AppendLine("Hello world!<br>").AppendLine(GraphDBs)).ToUTF8Bytes()
        //    };

        //}

        //#endregion

        //#region AllGraphDBs()

        //public override HTTPResponse AllGraphDBs()
        //{

        //    var GraphDBs = GraphServer.AllGraphDBs().
        //                               Select(t => "<a href=\"" + t.Uri + "\">" + t.Name + "</a>").
        //                               Aggregate((a, b) => a + "<br>" + b);

        //    return new HTTPResponseBuilder()
        //    {
        //        HTTPStatusCode = HTTPStatusCode.OK,
        //        ContentType    = HTTPContentType.HTML_UTF8,
        //        Content        = HTMLBuilder("www.graph-database.org v0.1", b => b.AppendLine("Hello world!<br>").AppendLine(GraphDBs)).ToUTF8Bytes()
        //    };

        //}

        //#endregion


        //#region (protected) VerticesSerialization(...)

        ///// <summary>
        ///// Serialize an enumeration of vertices.
        ///// </summary>
        ///// <param name="Vertex">A single vertex.</param>
        ///// <returns>The serialized vertex.</returns>
        //protected override Byte[] VerticesSerialization(IEnumerable<IGenericPropertyVertex<UInt64, Int64, String, String, Object,
        //                                                                            UInt64, Int64, String, String, Object,
        //                                                                            UInt64, Int64, String, String, Object,
        //                                                                            UInt64, Int64, String, String, Object>> Vertices)
        //{

        //    return HTMLBuilder("www.graph-database.org v0.1", b =>
        //    {

        //        b.AppendLine("Vertices<br>");

        //        foreach (var Vertex in Vertices)
        //        {

        //            foreach (var KVP in Vertex)
        //                b.Append(KVP.Key).
        //                  Append(" = ").
        //                  Append(KVP.Value.ToString()).
        //                  AppendLine("<br>");

        //            b.AppendLine("<br>");

        //        }

        //    }).ToUTF8Bytes();

        //}

        //#endregion

    }

}
