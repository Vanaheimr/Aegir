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

using System.Reflection;

using eu.Vanaheimr.Hermod.HTTP;

#endregion

namespace eu.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// EVENTSTREAM content representations.
    /// </summary>
    public class AegirTileService_EVENTSTREAM : AAegirTileService
    {

        #region Constructor(s)

        #region AegirTileService_EVENTSTREAM()

        /// <summary>
        /// Creates a new tile service for EVENTSTREAM content.
        /// </summary>
        public AegirTileService_EVENTSTREAM()
            : base(HTTPContentType.EVENTSTREAM)
        { }

        #endregion

        #region AegirTileService_ALL(IHTTPConnection)

        /// <summary>
        /// Creates a new tile service for EVENTSTREAM content.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public AegirTileService_EVENTSTREAM(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.EVENTSTREAM)
        {
            this.CallingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #endregion

    }

}

