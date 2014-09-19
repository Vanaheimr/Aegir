///*
// * Copyright (c) 2011-2013 Achim 'ahzf' Friedland <achim@graph-database.org>
// * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
// * 
// * This program is free software; you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation; either version 3 of the License, or
// * (at your option) any later version.
// * 
// * You may obtain a copy of the License at
// *   http://www.gnu.org/licenses/gpl.html
// * 
// * This program is distributed in the hope that it will be useful, but
// * WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// * General Public License for more details.
// */

//#region Usings

//using System;

//using org.GraphDefined.Vanaheimr.Hermod.HTTP;
//using org.GraphDefined.Vanaheimr.Hermod.Datastructures;
//using org.GraphDefined.Vanaheimr.Hermod;

//#endregion

//namespace org.GraphDefined.Vanaheimr.Aegir.Tiles
//{

//    /// <summary>
//    /// A tcp/http based Aegir map tiles server and/or
//    /// proxy which can also act as a AegirTilesClient.
//    /// </summary>
//    public class AegirTilesServer : AegirTilesClient
//    {

//        #region Data

//        private readonly HTTPServer<IAegirTilesService> HTTPServer;

//        #endregion

//        #region Properties

//        #region DefaultServerName

//        /// <summary>
//        /// The default server name.
//        /// </summary>
//        public String DefaultServerName
//        {
//            get
//            {
//                return "Vanaheimr Aegir Tiles HTTP Server v0.1";
//            }
//        }

//        #endregion

//        #endregion

//        #region Constructor(s)

//        #region AegirTilesServer()

//        /// <summary>
//        /// Initialize the TileServer using IPAddress.Any, http port 8182 and start the server.
//        /// </summary>
//        public AegirTilesServer()
//        {

//            this.HTTPServer = new HTTPServer<IAegirTilesService>(IPv4Address.Any, new IPPort(8182), Autostart: true){
//                                      ServerName = DefaultServerName
//                                  };

//            this.HTTPServer.OnNewHTTPService += TilesService => { TilesService.TilesServer = this; };

//        }

//        #endregion

//        #region AegirTilesServer(Port, AutoStart = true)

//        /// <summary>
//        /// Initialize the TileServer using IPAddress.Any and the given parameters.
//        /// </summary>
//        /// <param name="Port">The listening port</param>
//        /// <param name="Autostart"></param>
//        public AegirTilesServer(IPPort Port, Boolean Autostart = true)
//        {

//            this.HTTPServer = new HTTPServer<IAegirTilesService>(IPv4Address.Any, Port, Autostart: Autostart) {
//                                      ServerName = DefaultServerName
//                                  };

//            this.HTTPServer.OnNewHTTPService += TilesService => { TilesService.TilesServer = this; };

//        }

//        #endregion

//        #region AegirTilesServer(IIPAddress, Port, AutoStart = true)

//        /// <summary>
//        /// Initialize the HTTPServer using the given parameters.
//        /// </summary>
//        /// <param name="IIPAddress">The listening IP address(es)</param>
//        /// <param name="Port">The listening port</param>
//        /// <param name="Autostart"></param>
//        public AegirTilesServer(IIPAddress IIPAddress, IPPort Port, Boolean Autostart = true)
//        {

//            this.HTTPServer = new HTTPServer<IAegirTilesService>(IIPAddress, Port, Autostart: Autostart) {
//                                      ServerName = DefaultServerName
//                                  };

//            this.HTTPServer.OnNewHTTPService += TilesService => { TilesService.TilesServer = this; };

//        }

//        #endregion

//        #region AegirTilesServer(IPSocket, Autostart = true)

//        /// <summary>
//        /// Initialize the HTTPServer using the given parameters.
//        /// </summary>
//        /// <param name="IPSocket">The listening IPSocket.</param>
//        /// <param name="Autostart"></param>
//        public AegirTilesServer(IPSocket IPSocket, Boolean Autostart = true)
//        {

//            this.HTTPServer = new HTTPServer<IAegirTilesService>(IPSocket.IPAddress, IPSocket.Port, Autostart: Autostart) {
//                                      ServerName = DefaultServerName
//                                  };

//            this.HTTPServer.OnNewHTTPService += TilesService => { TilesService.TilesServer = this; };

//        }

//        #endregion

//        #endregion

//    }

//}
