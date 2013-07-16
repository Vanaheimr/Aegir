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
using System.Collections.Generic;

using eu.Vanaheimr.Hermod.HTTP;
using eu.Vanaheimr.Hermod.Datastructures;
using eu.Vanaheimr.Illias.Commons;

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// A tcp/http based Aegir tiles server.
    /// </summary>
    public class AegirTilesServer : HTTPServer<IAegirTilesService>,
                                    IAegirTilesServer

    {

        #region Data

        private readonly AutoDiscovery<IMapProvider> AutoMapProviders;

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
                return "Vanaheimr Aegir Tiles HTTP/REST Server v0.1";
            }
        }

        #endregion

        #region RegisteredMapProviderIds

        /// <summary>
        /// Return an enumeration of all registered map provider names.
        /// </summary>
        public IEnumerable<String> RegisteredMapProviderIds
        {
            get
            {
                return from   MapProvider
                       in     AutoMapProviders.RegisteredTypes
                       select MapProvider.Id;
            }
        }

        #endregion

        #region RegisteredMapProviders

        /// <summary>
        /// Return an enumeration of all registered map providers.
        /// </summary>
        public IDictionary<String, IMapProvider> RegisteredMapProviders
        {
            get
            {
                return AutoMapProviders.RegisteredTypes.ToDictionary(MapProvider => MapProvider.Id,
                                                                     MapProvider => MapProvider);
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region TileServer()

        /// <summary>
        /// Initialize the TileServer using IPAddress.Any, http port 8182 and start the server.
        /// </summary>
        public AegirTilesServer()
            : base(IPv4Address.Any, new IPPort(8182), Autostart: true)
        {

            ServerName       = DefaultServerName;
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true, Mapprovider => Mapprovider.Id);

            base.OnNewHTTPService += TilesService => { TilesService.TilesServer = this; };

        }

        #endregion

        #region TileServer(Port, AutoStart = true)

        /// <summary>
        /// Initialize the TileServer using IPAddress.Any and the given parameters.
        /// </summary>
        /// <param name="Port">The listening port</param>
        /// <param name="Autostart"></param>
        public AegirTilesServer(IPPort Port, Boolean Autostart = true)
            : base(IPv4Address.Any, Port, Autostart: Autostart)
        {

            ServerName       = DefaultServerName;
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true, Mapprovider => Mapprovider.Id);

            base.OnNewHTTPService += TilesService => { TilesService.TilesServer = this; };

        }

        #endregion

        #region TileServer(IIPAddress, Port, AutoStart = true)

        /// <summary>
        /// Initialize the HTTPServer using the given parameters.
        /// </summary>
        /// <param name="IIPAddress">The listening IP address(es)</param>
        /// <param name="Port">The listening port</param>
        /// <param name="Autostart"></param>
        public AegirTilesServer(IIPAddress IIPAddress, IPPort Port, Boolean Autostart = true)
            : base(IIPAddress, Port, Autostart: Autostart)
        {

            ServerName       = DefaultServerName;
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true, Mapprovider => Mapprovider.Id);

            base.OnNewHTTPService += TilesService => { TilesService.TilesServer = this; };

        }

        #endregion

        #region TileServer(IPSocket, Autostart = true)

        /// <summary>
        /// Initialize the HTTPServer using the given parameters.
        /// </summary>
        /// <param name="IPSocket">The listening IPSocket.</param>
        /// <param name="Autostart"></param>
        public AegirTilesServer(IPSocket IPSocket, Boolean Autostart = true)
            : base(IPSocket.IPAddress, IPSocket.Port, Autostart: Autostart)
        {

            ServerName       = DefaultServerName;
            AutoMapProviders = new AutoDiscovery<IMapProvider>(true, Mapprovider => Mapprovider.Id);

            base.OnNewHTTPService += TilesService => { TilesService.TilesServer = this; };

        }

        #endregion

        #endregion


        #region GetTile(MapProviderId, Zoom, X, Y)

        /// <summary>
        /// Return the tile for the given parameters.
        /// </summary>
        /// <param name="MapProviderId">The unique identification of the map provider.</param>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <param name="X">The x coordinate of the tile.</param>
        /// <param name="Y">The y coordinate of the tile.</param>
        /// <returns>The requested tile as an array of bytes.</returns>
        public Byte[] GetTile(String MapProviderId, UInt32 ZoomLevel, UInt32 X, UInt32 Y)
        {

            IMapProvider _MapProvider = null;
            if (AutoMapProviders.TryGetInstance(MapProviderId, out _MapProvider))
            {
                return _MapProvider.GetTile(ZoomLevel, X, Y);
            }

            return null;

        }

        #endregion


    }

}
