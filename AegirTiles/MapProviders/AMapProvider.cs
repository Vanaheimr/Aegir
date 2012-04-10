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
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace de.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// An abstract map provider.
    /// </summary>
    public abstract class AMapProvider : IMapProvider
    {

        #region Data

        /// <summary>
        /// The stored tiles.
        /// </summary>
        protected Byte[][][][] TileCache;

        #endregion

        #region Properties

        #region Name

        /// <summary>
        /// The unique name of this map provider.
        /// </summary>
        public String Name
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

        #region AMapProvider(Name, Description, Copyright, IsMemoryCacheable, MemoryCacheEnabled, UriPattern, Hosts = null)

        /// <summary>
        /// Creates an abstract map provider.
        /// </summary>
        /// <param name="Name">The unique name of this map provider.</param>
        /// <param name="Description">The description of this map provider.</param>
        /// <param name="InfoUri">An Uri to retrieve more information on this map provider.</param>
        /// <param name="Copyright">Copyright information on the content provided by this map provider.</param>
        /// <param name="IsMemoryCacheable">Whether this map provider allows to cache the retrieved tiles in memory or not.</param>
        /// <param name="MemoryCacheEnabled">Whether the memory cache is in use or not.</param>
        /// <param name="UriPattern">The Uri pattern of this map provider. This hase to contain placeholders for "zoom", "x" and "y".</param>
        /// <param name="Hosts">An enumeration of all hosts serving this mapping service.</param>
        public AMapProvider(String              Name,
                            String              Description,
                            String              InfoUri,
                            String              Copyright,
                            Boolean             IsMemoryCacheable,
                            Boolean             MemoryCacheEnabled,
                            String              UriPattern,
                            IEnumerable<String> Hosts = null)
        {

            this.Name               = Name;
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

            TileCache = new Byte[23][][][];

        }

        #endregion

        #endregion


        #region (virtual) GetTile(Zoom, X, Y)

        /// <summary>
        /// Return the tile for the given zoom, x and y parameters.
        /// </summary>
        /// <param name="Zoom">The zoom level.</param>
        /// <param name="X">The tile x-value.</param>
        /// <param name="Y">The tile y-value.</param>
        /// <returns>A stream containing the tile.</returns>
        public virtual Stream GetTile(UInt32 Zoom, UInt32 X, UInt32 Y)
        {
            
            var XCache = TileCache[Zoom];
            if (XCache == null)
            {
                XCache          = new Byte[(Int32) Math.Pow(2, Zoom)][][];
                TileCache[Zoom] = XCache;
            }

            var YCache = XCache[X];
            if (YCache == null)
            {
                YCache    = new Byte[(Int32) Math.Pow(2, Zoom)][];
                XCache[X] = YCache;
            }

            if (YCache[Y] == null)
            {

                foreach (var ActualHost in Hosts)
                {

                    var _Url = ActualHost +
                               UriPattern.Replace("{zoom}", Zoom.ToString()).
                                          Replace("{x}",       X.ToString()).
                                          Replace("{y}",       Y.ToString());

                    Debug.WriteLine("Fetching: " + _Url);

                    try
                    {
                        var wc = new WebClient();
                        wc.Proxy = null;
                        YCache[Y] = wc.DownloadData(_Url);
                    }
                    catch (Exception e)
                    {
                        
                        Debug.WriteLine("AMapProvider: " + e);
                        
                        // Try next host...
                        continue;

                    }

                    Debug.WriteLine("Fetched: " + _Url);
                    break;

                }

            }

            return new MemoryStream(YCache[Y]);

        }

        #endregion

    }

}
