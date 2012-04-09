/*
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
    /// EVENTSTREAM content representations.
    /// </summary>
    public class TileService_EVENTSTREAM : ATileService
    {

        #region Constructor(s)

        #region TileService_EVENTSTREAM()

        /// <summary>
        /// Creates a new tile service for EVENTSTREAM content.
        /// </summary>
        public TileService_EVENTSTREAM()
            : base(HTTPContentType.EVENTSTREAM)
        { }

        #endregion

        #region TileService_ALL(IHTTPConnection)

        /// <summary>
        /// Creates a new tile service for EVENTSTREAM content.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public TileService_EVENTSTREAM(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.EVENTSTREAM)
        {
            this.CallingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #endregion

    }

}

