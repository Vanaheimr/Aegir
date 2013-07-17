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
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using eu.Vanaheimr.Illias.Commons;
using eu.Vanaheimr.Illias.Commons.Collections;

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// A map tiles provider.
    /// </summary>
    public class MapTilesProvider
    {

        public class TileJob
        {

            public UInt32 ZoomLevel { get; private set; }
            public UInt32 X { get; private set; }
            public UInt32 Y { get; private set; }

            public TileJob(UInt32 ZoomLevel, UInt32 X, UInt32 Y)
            {
                this.ZoomLevel = ZoomLevel;
                this.X = X;
                this.Y = Y;
            }

            public override int GetHashCode()
            {
                return (ZoomLevel.ToString()+X.ToString()+Y.ToString()).GetHashCode();
            }

        }

        #region Data

        /// <summary>
        /// The stored tiles.
        /// </summary>
        protected MemoryStream[][][] TileCache;

        private ConcurrentDictionary<TileJob, Object> FetchIt;

        private ConcurrentDictionary<String, String> UrlHashes;

        #endregion

        #region Properties

        #region Id

        /// <summary>
        /// The unique identification of this map provider.
        /// </summary>
        public String Id
        {
            get;
            private set;
        }

        #endregion

        #region Description

        /// <summary>
        /// The description of this map provider.
        /// </summary>
        public String Description
        {
            get;
            private set;
        }

        #endregion

        #region Copyright

        /// <summary>
        /// Copyright information on the content
        /// provided by this map provider.
        /// </summary>
        public String Copyright
        {
            get;
            private set;
        }

        #endregion

        #region InfoUri

        /// <summary>
        /// An Uri to retrieve more information on this map provider.
        /// </summary>
        public String InfoUri
        {
            get;
            private set;
        }

        #endregion

        #region IsMemoryCacheable

        /// <summary>
        /// Whether this map provider allows to cache
        /// the retrieved tiles in memory or not.
        /// </summary>
        public Boolean IsMemoryCacheable
        {
            get;
            private set;
        }

        #endregion

        #region MemoryCacheEnabled

        /// <summary>
        /// Whether the memory cache is in use or not.
        /// </summary>
        public Boolean MemoryCacheEnabled
        {
            get;
            set;
        }

        #endregion

        #region UriPattern

        /// <summary>
        /// The Uri pattern of this map provider.
        /// This hase to contain placeholders for "zoom", "x" and "y".
        /// </summary>
        public String UriPattern
        {
            get;
            private set;
        }

        #endregion

        #region Hosts

        /// <summary>
        /// An enumeration of all hosts serving this mapping service.
        /// </summary>
        public IEnumerable<String> Hosts
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region MapTilesProvider(Id, Description, Copyright, IsMemoryCacheable, MemoryCacheEnabled, UriPattern, Hosts = null)

        /// <summary>
        /// Creates an abstract map provider.
        /// </summary>
        /// <param name="Id">The unique name of this map provider.</param>
        /// <param name="Description">The description of this map provider.</param>
        /// <param name="InfoUri">An Uri to retrieve more information on this map provider.</param>
        /// <param name="Copyright">Copyright information on the content provided by this map provider.</param>
        /// <param name="IsMemoryCacheable">Whether this map provider allows to cache the retrieved tiles in memory or not.</param>
        /// <param name="MemoryCacheEnabled">Whether the memory cache is in use or not.</param>
        /// <param name="UriPattern">The Uri pattern of this map provider. This hase to contain placeholders for "zoom", "x" and "y".</param>
        /// <param name="Hosts">An enumeration of all hosts serving this mapping service.</param>
        public MapTilesProvider(String               Id,
                                String               Description,
                                String               InfoUri,
                                String               Copyright,
                                Boolean              IsMemoryCacheable,
                                Boolean              MemoryCacheEnabled,
                                String               UriPattern,
                                IEnumerable<String>  Hosts = null)
        {

            this.Id                 = Id;
            this.Description        = Description;
            this.InfoUri            = InfoUri;
            this.Copyright          = Copyright;
            this.IsMemoryCacheable  = IsMemoryCacheable;
            this.MemoryCacheEnabled = MemoryCacheEnabled;
            this.UriPattern         = UriPattern;

            if (Hosts != null)
                this.Hosts = Hosts;
            else
                this.Hosts = new List<String>();

            TileCache = new MemoryStream[23][][];

            this.FetchIt = new ConcurrentDictionary<TileJob, Object>();
            this.UrlHashes = new ConcurrentDictionary<String, String>();

        }

        #endregion

        #endregion


        #region (virtual) GetTile(ZoomLevel, X, Y)

        /// <summary>
        /// Return the tile for the given ZoomLevel, X and Y coordinates.
        /// </summary>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <param name="X">The tile x-value.</param>
        /// <param name="Y">The tile y-value.</param>
        /// <returns>A stream containing the tile.</returns>
        public virtual Task<Tuple<MemoryStream, Object>> GetTile(UInt32 ZoomLevel, UInt32 X, UInt32 Y, Object State = null)
        {

            MemoryStream[] YCache = null;

            var XCache = TileCache[ZoomLevel];

            if (XCache == null)
                XCache = TileCache[ZoomLevel] = new MemoryStream[(Int32) Math.Pow(2, ZoomLevel)][];

            YCache = XCache[X];

            if (YCache == null)
                YCache = XCache[X] = new MemoryStream[(Int32) Math.Pow(2, ZoomLevel)];

            if (YCache[Y] == null)
            {

                YCache[Y] = new MemoryStream();

                var Urls = Hosts.Select(Host => Host + UriPattern.Replace("{zoom}", ZoomLevel.ToString()).
                                                                  Replace("{x}",    X.        ToString()).
                                                                  Replace("{y}",    Y.        ToString())).ToArray();

                #region Fetch Tiles

                return Task.Factory.StartNew<Tuple<MemoryStream, Object>>(Data => {

                    var _Urls          = (Data as Tuple<String[], MemoryStream, Object>).Item1;
                    var _MemoryStream  = (Data as Tuple<String[], MemoryStream, Object>).Item2;
                    var _State         = (Data as Tuple<String[], MemoryStream, Object>).Item3;
                    var TileBytes      = new Byte[0];

                    foreach (var Url in _Urls)
                    {

                        try
                        {

                            TileBytes = new WebClient() { Proxy = null }.
                                DownloadData(Url);

                            Debug.WriteLine("Fetched: " + Url);

                            break;

                        }
                        catch (Exception e)
                        {

                            Debug.WriteLine("MapTilesProvider Exception: " + e);

                            // Try next host...
                            continue;

                        }

                    }

                    _MemoryStream.Write(TileBytes, 0, TileBytes.Length);

                    return new Tuple<MemoryStream, Object>(_MemoryStream, _State);

                    },
                    new Tuple<String[], MemoryStream, Object>(Urls, YCache[Y], State),
                    TaskCreationOptions.AttachedToParent);

                #endregion

            }

            else
                return Task.Factory.StartNew<Tuple<MemoryStream, Object>>(Data => {

                    var _MemoryStream  = (Data as Tuple<MemoryStream, Object>).Item1;
                    var _State         = (Data as Tuple<MemoryStream, Object>).Item2;

                    _MemoryStream.Seek(0, SeekOrigin.Begin);

                    return new Tuple<MemoryStream, Object>(_MemoryStream, _State);

                    },
                    new Tuple<MemoryStream, Object>(YCache[Y], State),
                    TaskCreationOptions.AttachedToParent);

        }

        #endregion

    }

}
