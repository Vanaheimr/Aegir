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
using System.IO;
using System.Linq;
using System.Collections.Generic;

using eu.Vanaheimr.Illias.Commons;
using System.Threading.Tasks;

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// An Aegir tiles client.
    /// </summary>
    public class AegirTilesClient : IEnumerable<MapTilesProvider>
    {

        #region Data

        private readonly Dictionary<String, MapTilesProvider> MapProviders;

        #endregion

        #region Properties

        #region Current_MapTilesProvider

        private MapTilesProvider _CurrentProvider;

        public MapTilesProvider CurrentProvider
        {

            get
            {
                return _CurrentProvider;
            }

            set
            {
                SetMapProvider(value);
            }

        }

        #endregion

        #region CurrentProviderId

        public String CurrentProviderId
        {

            get
            {
                return _CurrentProvider.Id;
            }

            set
            {
                SetMapProvider(value);
            }

        }

        #endregion

        #region RegisteredMapProviderIds

        /// <summary>
        /// Return an enumeration of all registered map provider identifications.
        /// </summary>
        public IEnumerable<String> RegisteredMapProviderIds
        {
            get
            {
                return MapProviders.Select(MapProvider => MapProvider.Key);
            }
        }

        #endregion

        #region RegisteredMapProviders

        /// <summary>
        /// Return an enumeration of all registered map providers.
        /// </summary>
        public IDictionary<String, MapTilesProvider> RegisteredMapProviders
        {
            get
            {
                return MapProviders;
            }
        }

        #endregion

        #endregion

        #region Events

        #region MapProviderAdded

        /// <summary>
        /// An event handler getting fired whenever a
        /// map provider was added.
        /// </summary>
        public delegate void MapProviderAddedEventHandler(AegirTilesClient Sender, MapTilesProvider NewMapProvider);

        /// <summary>
        /// An event getting fired whenever a map
        /// provider was added.
        /// </summary>
        public event MapProviderAddedEventHandler MapProviderAdded;

        #endregion

        #region MapProviderChanged

        /// <summary>
        /// An event handler getting fired whenever the
        /// map provider of the map changed.
        /// </summary>
        public delegate void MapProviderChangedEventHandler(AegirTilesClient Sender, MapTilesProvider OldMapProvider, MapTilesProvider NewMapProvider);

        /// <summary>
        /// An event getting fired whenever the map provider
        /// of the map changed.
        /// </summary>
        public event MapProviderChangedEventHandler MapProviderChanged;

        #endregion

        #region MapProviderRemoved

        /// <summary>
        /// An event handler getting fired whenever a
        /// map provider was removed.
        /// </summary>
        public delegate void MapProviderRemovedEventHandler(AegirTilesClient Sender, MapTilesProvider RemovedMapProvider);

        /// <summary>
        /// An event getting fired whenever a map
        /// provider was removed.
        /// </summary>
        public event MapProviderRemovedEventHandler MapProviderRemoved;

        #endregion

        #endregion

        #region Constructor(s)

        #region AegirTilesClient(params MapProviders)

        /// <summary>
        /// Create a new tile client.
        /// </summary>
        public AegirTilesClient(params MapTilesProvider[] MapProviders)
        {

            this.MapProviders = new Dictionary<String, MapTilesProvider>();

            MapProviders.ForEach(MapProvider => this.MapProviders.Add(MapProvider.Id, MapProvider));

            if (MapProviders.Count() > 0)
                CurrentProviderId = MapProviders.First().Id;

            //AutoMapProviders = new AutoDiscovery<IMapTilesProvider>(true, Mapprovider => Mapprovider.Id);

        }

        #endregion

        #endregion


        public AegirTilesClient Register(MapTilesProvider MapProvider, Boolean Activate = false)
        {

            if (MapProviders.ContainsKey(MapProvider.Id))
                throw new ArgumentException("Duplicate map tiles provider!");

            MapProviders.Add(MapProvider.Id, MapProvider);

            if (MapProviderAdded != null)
                MapProviderAdded(this, MapProvider);

            if (Activate || MapProviders.Count == 1)
                SetMapProvider(MapProvider);

            return this;

        }

        public void Remove(String MapProviderId)
        {

            if (!MapProviders.ContainsKey(MapProviderId))
                throw new ArgumentException("Unknown map tiles provider!");

            var OldProvider  = _CurrentProvider;

            MapProviders.Remove(MapProviderId);

            if (MapProviderRemoved != null)
                MapProviderRemoved(this, OldProvider);

            if (MapProviders.Count > 0)
                SetMapProvider(MapProviders.First().Value);

        }


        private void SetMapProvider(MapTilesProvider MapProvider)
        {

            if (!MapProviders.ContainsKey(MapProvider.Id))
                throw new ArgumentException("Unknown map tiles provider!");

            var OldProvider  = _CurrentProvider;
            _CurrentProvider = MapProvider;

            if (MapProviderChanged != null)
                MapProviderChanged(this, OldProvider, _CurrentProvider);

        }

        private void SetMapProvider(String MapProviderId)
        {

            if (!MapProviders.ContainsKey(MapProviderId))
                throw new ArgumentException("Unknown map tiles provider!");

            SetMapProvider(MapProviders[MapProviderId]);

        }


        #region GetTile(ZoomLevel, X, Y)

        /// <summary>
        /// Return the tile for the given parameters.
        /// </summary>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <param name="X">The x coordinate of the tile.</param>
        /// <param name="Y">The y coordinate of the tile.</param>
        /// <returns>The requested tile as an array of bytes.</returns>
        public Task<Tuple<Byte[], Object>> GetTile(UInt32 ZoomLevel, UInt32 X, UInt32 Y, Object State = null)
        {
            return CurrentProvider.GetTile(ZoomLevel, X, Y, State);
        }

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
        public Task<Tuple<Byte[], Object>> GetTile(String MapProviderId, UInt32 ZoomLevel, UInt32 X, UInt32 Y, Object State = null)
        {

            MapTilesProvider MapProvider = null;

            if (MapProviders.TryGetValue(MapProviderId, out MapProvider))
            {
                return MapProvider.GetTile(ZoomLevel, X, Y, State);
            }

            return null;

        }

        #endregion



        public IEnumerator<MapTilesProvider> GetEnumerator()
        {

            return this.MapProviders.
                            Select(MapProvider => MapProvider.Value).
                            GetEnumerator();

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {

            return this.MapProviders.
                            Select(MapProvider => MapProvider.Value).
                            GetEnumerator();

        }

    }

}
