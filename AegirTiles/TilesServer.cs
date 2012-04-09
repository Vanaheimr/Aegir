/*
 * Copyright (c) 2010-2012 Achim 'ahzf' Friedland <achim.friedland@aperis.com>
 * This file is part of TileServer <http://www.github.com/ahzf/TileServer>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using de.ahzf.Hermod.HTTP;
using de.ahzf.Hermod.Datastructures;
using de.ahzf.Illias.Commons;

#endregion

namespace de.ahzf.TileServer
{

    /// <summary>
    /// A tcp/http based TileServer.
    /// </summary>
    public class TileServer : HTTPServer<ITileService>
    {

        #region Data

        private AutoDiscovery<IMapProvider> AutoMapProviders;

        #endregion
        
        #region Properties

        #region DefaultServerName

        /// <summary>
        /// The default server name.
        /// </summary>
        public override String DefaultServerName
        {
            get
            {
                return "Linked Energy Tile HTTP/REST Server v0.1";
            }
        }

        #endregion

        #region MapProviders

        private IDictionary<String, IMapProvider> _MapProviders;

        /// <summary>
        /// An enumeration of all map providers.
        /// </summary>
        public IDictionary<String, IMapProvider> MapProviders
        {
            get
            {

                if (_MapProviders != null)
                    return _MapProviders;

                _MapProviders = AutoMapProviders.RegisteredTypes.ToDictionary(MapProvider => MapProvider.Name,
                                                                              MapProvider => MapProvider);

                return _MapProviders;

            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region TileServer()

        /// <summary>
        /// Initialize the TileServer using IPAddress.Any, http port 8182 and start the server.
        /// </summary>
        public TileServer()
            : base(IPv4Address.Any, new IPPort(8182), Autostart: true)
        {

            ServerName       = DefaultServerName;
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true);

            base.OnNewHTTPService += TilesService => { TilesService.TileServer = this; };

        }

        #endregion

        #region TileServer(Port, AutoStart = false)

        /// <summary>
        /// Initialize the TileServer using IPAddress.Any and the given parameters.
        /// </summary>
        /// <param name="Port">The listening port</param>
        /// <param name="Autostart"></param>
        public TileServer(IPPort Port, Boolean Autostart = false)
            : base(IPv4Address.Any, Port, Autostart: true)
        {

            ServerName       = DefaultServerName;
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true);

            base.OnNewHTTPService += TilesService => { TilesService.TileServer = this; };

        }

        #endregion

        #region TileServer(IIPAddress, Port, AutoStart = false)

        /// <summary>
        /// Initialize the HTTPServer using the given parameters.
        /// </summary>
        /// <param name="IIPAddress">The listening IP address(es)</param>
        /// <param name="Port">The listening port</param>
        /// <param name="Autostart"></param>
        public TileServer(IIPAddress IIPAddress, IPPort Port, Boolean Autostart = false)
            : base(IIPAddress, Port, Autostart: true)
        {

            ServerName       = DefaultServerName;
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true);

            base.OnNewHTTPService += TilesService => { TilesService.TileServer = this; };

        }

        #endregion

        #region TileServer(IPSocket, Autostart = false)

        /// <summary>
        /// Initialize the HTTPServer using the given parameters.
        /// </summary>
        /// <param name="IPSocket">The listening IPSocket.</param>
        /// <param name="Autostart"></param>
        public TileServer(IPSocket IPSocket, Boolean Autostart = false)
            : base(IPSocket.IPAddress, IPSocket.Port, Autostart: true)
        {

            ServerName       = DefaultServerName;
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true);

            base.OnNewHTTPService += TilesService => { TilesService.TileServer = this; };

        }

        #endregion

        #endregion


        public Stream GetTile(String MapProviderName, UInt32 Zoom, UInt32 X, UInt32 Y)
        {
            
            IMapProvider _MapProvider = null;
            if (MapProviders.TryGetValue(MapProviderName, out _MapProvider))
            {
                return _MapProvider.GetTile(Zoom, X, Y);
            }

            return null;

        }


        #region ToString()

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        public override string ToString()
        {
            return "TileServer";
        }

        #endregion

    }

}
