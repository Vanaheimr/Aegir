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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// A map tiles provider.
    /// </summary>
    public class MapTilesProvider
    {

        #region Data

        /// <summary>
        /// The cached map tiles.
        /// </summary>
        protected MemoryStream[][][] TileCache;

        #endregion

        #region Properties

        #region Id

        /// <summary>
        /// The unique identification of this map provider.
        /// </summary>
        public String Id    { get; private set; }

        #endregion

        #region Description

        /// <summary>
        /// The description of this map provider.
        /// </summary>
        public String Description   { get; private set; }

        #endregion

        #region Copyright

        /// <summary>
        /// Copyright information on the content
        /// provided by this map provider.
        /// </summary>
        public String Copyright     { get; private set; }

        #endregion

        #region InfoUri

        /// <summary>
        /// An Uri to retrieve more information on this map provider.
        /// </summary>
        public String InfoUri   { get; private set; }

        #endregion

        #region IsMemoryCacheable

        /// <summary>
        /// Whether this map provider allows to cache
        /// the retrieved tiles in memory or not.
        /// </summary>
        public Boolean IsMemoryCacheable    { get; private set; }

        #endregion

        #region MemoryCacheEnabled

        /// <summary>
        /// Whether the memory cache is in use or not.
        /// </summary>
        public Boolean MemoryCacheEnabled   { get; set; }

        #endregion

        #region ZoomRange

        /// <summary>
        /// The valid range of zoom levels for this map.
        /// </summary>
        public Range<Byte> ZoomRange { get; private set; }

        #endregion

        #region UriPatterns

        /// <summary>
        /// An enumeration of all URIs serving this map tiles service.
        /// These strings should contain placeholders for the "zoom", "x" and "y" parameters.
        /// </summary>
        /// <example>http://tile.openstreetmap.org/{zoom}/{x}/{y}.png</example>
        public IEnumerable<String> UriPatterns  { get; private set; }

        #endregion

        #endregion

        #region Constructor(s)

        #region MapTilesProvider(Id, Description, InfoUri, Copyright, IsMemoryCacheable, MemoryCacheEnabled, ZoomRange, UriPatterns)

        /// <summary>
        /// Creates an abstract map provider.
        /// </summary>
        /// <param name="Id">The unique name of this map provider.</param>
        /// <param name="Description">The description of this map provider.</param>
        /// <param name="InfoUri">An Uri to retrieve more information on this map provider.</param>
        /// <param name="Copyright">Copyright information on the content provided by this map provider.</param>
        /// <param name="IsMemoryCacheable">Whether this map provider allows to cache the retrieved tiles in memory or not.</param>
        /// <param name="MemoryCacheEnabled">Whether the memory cache is in use or not.</param>
        /// <param name="ZoomRange">The valid range of zoom levels for this map.</param>
        /// <param name="UriPatterns">An enumeration of all URIs serving this map tiles service. hese strings should contain placeholders for the "zoom", "x" and "y" parameters.</param>
        public MapTilesProvider(String               Id,
                                String               Description,
                                String               InfoUri,
                                String               Copyright,
                                Boolean              IsMemoryCacheable,
                                Boolean              MemoryCacheEnabled,
                                Range<Byte>          ZoomRange,
                                IEnumerable<String>  UriPatterns)

        {

            this.Id                  = Id;
            this.Description         = Description;
            this.InfoUri             = InfoUri;
            this.Copyright           = Copyright;
            this.IsMemoryCacheable   = IsMemoryCacheable;
            this.MemoryCacheEnabled  = MemoryCacheEnabled;
            this.ZoomRange           = ZoomRange;
            this.UriPatterns         = (UriPatterns != null) ? UriPatterns : new List<String>();

            this.TileCache           = new MemoryStream[ZoomRange.Max][][];

        }

        #endregion

        #endregion


        #region (virtual) GetTile<T>(ZoomLevel, X, Y)

        /// <summary>
        /// Return the tile for the given ZoomLevel, X and Y coordinates.
        /// </summary>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <param name="X">The tile x-value.</param>
        /// <param name="Y">The tile y-value.</param>
        /// <param name="State">Some state to be returned with the tile, e.g. the screen coordinate where to paint it.</param>
        /// <returns>A stream containing the tile.</returns>
        public virtual Task<Tuple<MemoryStream, T>> GetTile<T>(UInt32  ZoomLevel,
                                                               UInt32  X,
                                                               UInt32  Y,
                                                               T       State = default(T))
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

                var Urls = UriPatterns.Select(Uri => Uri.Replace("{zoom}", ZoomLevel.ToString()).
                                                         Replace("{x}",    X.        ToString()).
                                                         Replace("{y}",    Y.        ToString())).ToArray();

                #region Fetch Tiles

                return Task.Factory.StartNew<Tuple<MemoryStream, T>>(Data => {

                    var _Urls          = (Data as Tuple<String[], MemoryStream, T>).Item1;
                    var _MemoryStream  = (Data as Tuple<String[], MemoryStream, T>).Item2;
                    var _State         = (Data as Tuple<String[], MemoryStream, T>).Item3;
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

                    return new Tuple<MemoryStream, T>(_MemoryStream, _State);

                    },
                    new Tuple<String[], MemoryStream, T>(Urls, YCache[Y], State),
                    TaskCreationOptions.AttachedToParent);

                #endregion

            }

            else
                return Task.Factory.StartNew<Tuple<MemoryStream, T>>(Data => {

                    var _MemoryStream  = (Data as Tuple<MemoryStream, T>).Item1;
                    var _State         = (Data as Tuple<MemoryStream, T>).Item2;

                    _MemoryStream.Seek(0, SeekOrigin.Begin);

                    return new Tuple<MemoryStream, T>(_MemoryStream, _State);

                    },
                    new Tuple<MemoryStream, T>(YCache[Y], State),
                    TaskCreationOptions.AttachedToParent);

        }

        #endregion

    }

}
