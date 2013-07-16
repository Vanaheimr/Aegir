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
    /// ALL content representations.
    /// </summary>
    public class AegirTileService_ALL : AAegirTileService
    {

        #region Constructor(s)

        #region AegirTileService_ALL()

        /// <summary>
        /// Creates a new tile service for all http content types.
        /// </summary>
        public AegirTileService_ALL()
            : base(HTTPContentType.ALL)
        { }

        #endregion

        #region AegirTileService_ALL(IHTTPConnection)

        /// <summary>
        /// Creates a new tile service for all http content types.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public AegirTileService_ALL(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.ALL)
        {
            this.CallingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #endregion

    }

}

