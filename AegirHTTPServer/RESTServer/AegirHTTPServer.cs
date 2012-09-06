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

using de.ahzf.Vanaheimr.Bifrost.HTTP.Server;
using de.ahzf.Vanaheimr.Hermod.Datastructures;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.HTTPServer
{

    /// <summary>
    /// A TCP/HTTP/REST based LinkedEnergy server.
    /// </summary>
    public class AegirHTTPServer : BifrostHTTPServer<IAegirHTTPService>,
                                   IAegirHTTPServer
    {

        #region AegirHTTPServer()

        /// <summary>
        /// Initialize the HTTP server using IPAddress.Any, http port 8182 and start the server.
        /// </summary>
        public AegirHTTPServer()
        {
            base.OnNewHTTPService += IAegirHTTPService => { IAegirHTTPService.GraphServer = this; };
        }

        #endregion

        #region AegirHTTPServer(Port, AutoStart = true)

        /// <summary>
        /// Initialize the HTTP server using IPAddress.Any and the given parameters.
        /// </summary>
        /// <param name="Port">The listening port</param>
        /// <param name="Autostart"></param>
        public AegirHTTPServer(IPPort  Port,
                                  Boolean Autostart = true)
            : base(Port, Autostart)
        
        {
            base.OnNewHTTPService += IAegirHTTPService => { IAegirHTTPService.GraphServer = this; };
        }

        #endregion

        #region AegirHTTPServer(IIPAddress, Port, AutoStart = true)

        /// <summary>
        /// Initialize the HTTP server using the given parameters.
        /// </summary>
        /// <param name="IIPAddress">The listening IP address(es)</param>
        /// <param name="Port">The listening port</param>
        /// <param name="Autostart"></param>
        public AegirHTTPServer(IIPAddress IIPAddress,
                                  IPPort     Port,
                                  Boolean    Autostart = false)
            : base(IIPAddress, Port, Autostart)

        {
            base.OnNewHTTPService += IAegirHTTPService => { IAegirHTTPService.GraphServer = this; };
        }

        #endregion

        #region AegirHTTPServer(IPSocket, Autostart = true)

        /// <summary>
        /// Initialize the HTTP server using the given parameters.
        /// </summary>
        /// <param name="IPSocket">The listening IPSocket.</param>
        /// <param name="Autostart"></param>
        public AegirHTTPServer(IPSocket IPSocket,
                                  Boolean  Autostart = true)
            : base(IPSocket.IPAddress, IPSocket.Port, Autostart)

        {
            base.OnNewHTTPService += IAegirHTTPService => { IAegirHTTPService.GraphServer = this; };
        }

        #endregion

    }

}
