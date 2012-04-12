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
using System.Collections.Generic;
using System.IO;

#endregion

namespace de.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// The interface for every map provider
    /// </summary>
    public interface IMapProvider
    {

        /// <summary>
        /// The unique name of this map provider.
        /// </summary>
        String              Name                { get; }

        /// <summary>
        /// The description of this map provider.
        /// </summary>
        String              Description         { get; }

        /// <summary>
        /// Whether this map provider allows to cache
        /// the retrieved tiles in memory or not.
        /// </summary>
        Boolean             IsMemoryCacheable   { get; }

        /// <summary>
        /// Whether the memory cache is in use or not.
        /// </summary>
        Boolean             MemoryCacheEnabled  { get; }

        /// <summary>
        /// The Uri pattern of this map provider.
        /// This hase to contain placeholders for "zoom", "x" and "y".
        /// </summary>
        String              UriPattern          { get; }

        /// <summary>
        /// An enumeration of all hosts serving this mapping service.
        /// </summary>
        IEnumerable<String> Hosts               { get; }



        Byte[] GetTile(UInt32 Zoom, UInt32 X, UInt32 Y);

        Stream GetTileStream(UInt32 ZoomLevel, UInt32 X, UInt32 Y);

    }

}
