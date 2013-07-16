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
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// The interface for every map provider
    /// </summary>
    public interface IMapProvider
    {

        /// <summary>
        /// The unique identification of this map provider.
        /// </summary>
        String              Id                  { get; }

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
