﻿///*
// * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <code@ahzf.de>
// * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using System;
//using System.Text;
//using System.Linq;
//using System.Reflection;

//using org.GraphDefined.Vanaheimr.Illias;
//using org.GraphDefined.Vanaheimr.Illias.Collections;
//using org.GraphDefined.Vanaheimr.Balder;
//using org.GraphDefined.Vanaheimr.Hermod.HTTP;
//using org.GraphDefined.Vanaheimr.Bifrost.HTTP.Server;

//#endregion

//namespace org.GraphDefined.Vanaheimr.Aegir.HTTPServer
//{

//    /// <summary>
//    /// XML content representation.
//    /// </summary>
//    public class AegirHTTPService_XML<TIdVertex,    TRevIdVertex,    TVertexLabel,    TKeyVertex,    TValueVertex,
//                                      TIdEdge,      TRevIdEdge,      TEdgeLabel,      TKeyEdge,      TValueEdge,
//                                      TIdMultiEdge, TRevIdMultiEdge, TMultiEdgeLabel, TKeyMultiEdge, TValueMultiEdge,
//                                      TIdHyperEdge, TRevIdHyperEdge, THyperEdgeLabel, TKeyHyperEdge, TValueHyperEdge>

//                                      : AAegirHTTPService<TIdVertex,    TRevIdVertex,    TVertexLabel,    TKeyVertex,    TValueVertex,
//                                                          TIdEdge,      TRevIdEdge,      TEdgeLabel,      TKeyEdge,      TValueEdge,
//                                                          TIdMultiEdge, TRevIdMultiEdge, TMultiEdgeLabel, TKeyMultiEdge, TValueMultiEdge,
//                                                          TIdHyperEdge, TRevIdHyperEdge, THyperEdgeLabel, TKeyHyperEdge, TValueHyperEdge>

//        where TIdVertex        : IEquatable<TIdVertex>,       IComparable<TIdVertex>,       IComparable, TValueVertex
//        where TIdEdge          : IEquatable<TIdEdge>,         IComparable<TIdEdge>,         IComparable, TValueEdge
//        where TIdMultiEdge     : IEquatable<TIdMultiEdge>,    IComparable<TIdMultiEdge>,    IComparable, TValueMultiEdge
//        where TIdHyperEdge     : IEquatable<TIdHyperEdge>,    IComparable<TIdHyperEdge>,    IComparable, TValueHyperEdge

//        where TRevIdVertex     : IEquatable<TRevIdVertex>,    IComparable<TRevIdVertex>,    IComparable, TValueVertex
//        where TRevIdEdge       : IEquatable<TRevIdEdge>,      IComparable<TRevIdEdge>,      IComparable, TValueEdge
//        where TRevIdMultiEdge  : IEquatable<TRevIdMultiEdge>, IComparable<TRevIdMultiEdge>, IComparable, TValueMultiEdge
//        where TRevIdHyperEdge  : IEquatable<TRevIdHyperEdge>, IComparable<TRevIdHyperEdge>, IComparable, TValueHyperEdge

//        where TVertexLabel     : IEquatable<TVertexLabel>,    IComparable<TVertexLabel>,    IComparable, TValueVertex
//        where TEdgeLabel       : IEquatable<TEdgeLabel>,      IComparable<TEdgeLabel>,      IComparable, TValueEdge
//        where TMultiEdgeLabel  : IEquatable<TMultiEdgeLabel>, IComparable<TMultiEdgeLabel>, IComparable, TValueMultiEdge
//        where THyperEdgeLabel  : IEquatable<THyperEdgeLabel>, IComparable<THyperEdgeLabel>, IComparable, TValueHyperEdge

//        where TKeyVertex       : IEquatable<TKeyVertex>,      IComparable<TKeyVertex>,      IComparable
//        where TKeyEdge         : IEquatable<TKeyEdge>,        IComparable<TKeyEdge>,        IComparable
//        where TKeyMultiEdge    : IEquatable<TKeyMultiEdge>,   IComparable<TKeyMultiEdge>,   IComparable
//        where TKeyHyperEdge    : IEquatable<TKeyHyperEdge>,   IComparable<TKeyHyperEdge>,   IComparable

//    {

//        #region Constructor(s)

//        #region AegirHTTPService_XML()

//        /// <summary>
//        /// Creates a new Aegir HTTP service.
//        /// </summary>
//        public AegirHTTPService_XML()
//            : base(HTTPContentType.XML_UTF8)
//        { }

//        #endregion

//        #region AegirHTTPService_XML(IHTTPConnection)

//        /// <summary>
//        /// Creates a new Aegir HTTP service.
//        /// </summary>
//        /// <param name="IHTTPConnection">The http connection for this request.</param>
//        public AegirHTTPService_XML(IHTTPConnection IHTTPConnection)
//            : base(IHTTPConnection, HTTPContentType.XML_UTF8)
//        {
//            this.CallingAssembly = Assembly.GetExecutingAssembly();
//        }

//        #endregion

//        #endregion


//        #region GET_MapElementsWithinBoundingBox_protected(GraphId, Latitude1, Longitude1, Latitude2, Longitude2)

//        /// <summary>
//        /// Get all elements within the given bounding box.
//        /// </summary>
//        /// <param name="GraphId">The unique identification of the graph.</param>
//        /// <param name="Latitude1">The first latitude.</param>
//        /// <param name="Longitude1">The first longitude.</param>
//        /// <param name="Latitude2">The second latitude.</param>
//        /// <param name="Longitude2">The second longitude.</param>
//        public override HTTPResponse GET_VerticesInArea(String GraphId, String Latitude1, String Longitude1, String Latitude2, String Longitude2)
//        {

//            var StringBuilder  = new StringBuilder();
//            var Result         = base.GET_VerticesInArea_protected(GraphId, Latitude1, Longitude1, Latitude2, Longitude2);

//            if (Result.HasErrors)
//                return Result.Error;

//            if (Result.Data.Any())
//            {

//                StringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
//                StringBuilder.AppendLine("<results>");

//                Result.Data.ForEach(Vertex => StringBuilder.Append("<properties>").
//                                                            Append("<id>").          Append(Vertex.Id.ToString()).Append("</id>").
//                                                            Append("<label>").       Append(Vertex.Label.ToString()).Append("</label>").
//                                                            Append("<description>"). Append(VertexPropertyKeyParser("Description")).Append("</description>").
//                                                            Append("<latitude>").    Append(Vertex.GetDouble(LatitudeKey). ToString().Replace(",",".")).Append("</latitude>").
//                                                            Append("<longitude>").   Append(Vertex.GetDouble(LongitudeKey).ToString().Replace(",",".")).Append("</longitude>").
//                                                            AppendLine("</properties>"));

//                StringBuilder.AppendLine("</results>");
//                StringBuilder.AppendLine("</xml>");

//            }

//            return new HTTPResponseBuilder() {
//                HTTPStatusCode = HTTPStatusCode.OK,
//                ContentType    = this.HTTPContentTypes.First(),
//                Content        = StringBuilder.ToString().ToUTF8Bytes()
//            };

//        }

//        #endregion


//    }

//}
