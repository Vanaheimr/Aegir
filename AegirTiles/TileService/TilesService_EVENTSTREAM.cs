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

using System.Reflection;

using de.ahzf.Hermod.HTTP;

#endregion

namespace de.Vanaheimr.Aegir.Tiles
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

