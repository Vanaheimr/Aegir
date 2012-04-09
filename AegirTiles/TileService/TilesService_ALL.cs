﻿/*
 * Copyright (c) 2010-2012 Achim 'ahzf' Friedland <achim.friedland@aperis.com>
 * This file is part of TileServer <http://www.github.com/ahzf/TileServer>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Reflection;

using de.ahzf.Hermod.HTTP;

#endregion

namespace de.ahzf.TileServer
{

    /// <summary>
    /// ALL content representations.
    /// </summary>
    public class TileService_ALL : ATileService
    {

        #region Constructor(s)

        #region TileService_ALL()

        /// <summary>
        /// Creates a new tile service for all contents.
        /// </summary>
        public TileService_ALL()
            : base(HTTPContentType.ALL)
        { }

        #endregion

        #region TileService_ALL(IHTTPConnection)

        /// <summary>
        /// Creates a new tile service for all contents.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public TileService_ALL(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.ALL)
        {
            this.CallingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #endregion

    }

}

