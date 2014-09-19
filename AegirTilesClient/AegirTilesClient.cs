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
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir.Tiles
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

        #region CurrentProvider

        private MapTilesProvider _CurrentProvider;

        /// <summary>
        /// The current map tiles provider.
        /// </summary>
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

        /// <summary>
        /// The unique identification of the current map tiles provider.
        /// </summary>
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

        #region Providers

        /// <summary>
        /// Return an enumeration of all registered map provider identifications.
        /// </summary>
        public IEnumerable<MapTilesProvider> Providers
        {
            get
            {
                return MapProviders.
                           Select(MapProvider => MapProvider.Value);
            }
        }

        #endregion

        #region ProviderIds

        /// <summary>
        /// Return an enumeration of all registered map provider identifications.
        /// </summary>
        public IEnumerable<String> ProviderIds
        {
            get
            {
                return MapProviders.
                           Select(MapProvider => MapProvider.Key);
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

        }

        #endregion

        #endregion


        #region Register(MapTilesProvider, Activate = false)

        /// <summary>
        /// Register a new map tiles provider.
        /// </summary>
        /// <param name="MapTilesProvider">A map tiles provider.</param>
        /// <param name="Activate">Set the new map tiles provider as default.</param>
        public AegirTilesClient Register(MapTilesProvider  MapTilesProvider,
                                         Boolean           Activate = false)
        {

            if (MapProviders.ContainsKey(MapTilesProvider.Id))
                throw new ArgumentException("Duplicate map tiles provider!");

            MapProviders.Add(MapTilesProvider.Id, MapTilesProvider);

            if (MapProviderAdded != null)
                MapProviderAdded(this, MapTilesProvider);

            if (Activate || MapProviders.Count == 1)
                SetMapProvider(MapTilesProvider);

            return this;

        }

        #endregion


        public MapTilesProvider GetProvider(String Id)
        {

            MapTilesProvider MapTilesProvider;

            if (this.MapProviders.TryGetValue(Id, out MapTilesProvider))
                return MapTilesProvider;

            return null;

        }

        public Boolean TryGetProvider(String Id, out MapTilesProvider MapTilesProvider)
        {
            return this.MapProviders.TryGetValue(Id, out MapTilesProvider);
        }

        #region Remove(MapTilesProviderId)

        public void Remove(String MapTilesProviderId)
        {

            if (!MapProviders.ContainsKey(MapTilesProviderId))
                throw new ArgumentException("Unknown map tiles provider!");

            var MapTilesProvider = MapProviders[MapTilesProviderId];

            MapProviders.Remove(MapTilesProviderId);

            if (MapProviderRemoved != null)
                MapProviderRemoved(this, MapTilesProvider);

            if (MapProviders.Count > 0)
                SetMapProvider(MapProviders.First().Value);

        }

        #endregion

        #region Remove(MapTilesProvider)

        public void Remove(MapTilesProvider MapTilesProvider)
        {

            if (!MapProviders.ContainsValue(MapTilesProvider))
                throw new ArgumentException("Unknown map tiles provider!");

            MapProviders.Remove(MapTilesProvider.Id);

            if (MapProviderRemoved != null)
                MapProviderRemoved(this, MapTilesProvider);

            if (MapProviders.Count > 0)
                SetMapProvider(MapProviders.First().Value);

        }

        #endregion


        #region (private) SetMapProvider(MapProvider)

        private void SetMapProvider(MapTilesProvider MapProvider)
        {

            if (!MapProviders.ContainsKey(MapProvider.Id))
                throw new ArgumentException("Unknown map tiles provider!");

            var OldProvider  = _CurrentProvider;
            _CurrentProvider = MapProvider;

            if (MapProviderChanged != null)
                MapProviderChanged(this, OldProvider, _CurrentProvider);

        }

        #endregion

        #region (private) SetMapProvider(MapProviderId)

        private void SetMapProvider(String MapProviderId)
        {

            if (!MapProviders.ContainsKey(MapProviderId))
                throw new ArgumentException("Unknown map tiles provider!");

            SetMapProvider(MapProviders[MapProviderId]);

        }

        #endregion


        #region GetTile(ZoomLevel, X, Y, State = default(T))

        /// <summary>
        /// Return the tile for the given parameters.
        /// </summary>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <param name="X">The x coordinate of the tile.</param>
        /// <param name="Y">The y coordinate of the tile.</param>
        /// <param name="State">Some state to be returned with the tile, e.g. the screen coordinate where to paint it.</param>
        public Task<Tuple<MemoryStream, T>> GetTile<T>(UInt32 ZoomLevel,
                                                       UInt32 X,
                                                       UInt32 Y,
                                                       T      State = default(T))
        {
            return CurrentProvider.GetTile<T>(ZoomLevel, X, Y, State);
        }

        #endregion

        #region GetTile(MapProviderId, ZoomLevel, X, Y, State = default(T))

        /// <summary>
        /// Return the tile for the given parameters.
        /// </summary>
        /// <param name="MapProviderId">The unique identification of the map provider.</param>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <param name="X">The x coordinate of the tile.</param>
        /// <param name="Y">The y coordinate of the tile.</param>
        /// <param name="State">Some state to be returned with the tile, e.g. the screen coordinate where to paint it.</param>
        public Task<Tuple<MemoryStream, T>> GetTile<T>(String  MapProviderId,
                                                       UInt32  ZoomLevel,
                                                       UInt32  X,
                                                       UInt32  Y,
                                                       T       State = default(T))
        {

            MapTilesProvider MapProvider = null;

            if (MapProviders.TryGetValue(MapProviderId, out MapProvider))
            {
                return MapProvider.GetTile(ZoomLevel, X, Y, State);
            }

            return null;

        }

        #endregion


        #region GetEnumerator()

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

        #endregion

    }

}
