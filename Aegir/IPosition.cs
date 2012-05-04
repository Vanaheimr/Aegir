/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <achim@graph-database.org>
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

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// The interface for anything having a position.
    /// </summary>
    public interface IPosition : IReadonlyPosition
    {

        /// <summary>
        /// The latitude of something.
        /// </summary>
        Double  Latitude       { get; set; }

        /// <summary>
        /// The longitude of something.
        /// </summary>
        Double  Longitude      { get; set; }

        /// <summary>
        /// The altitude of something.
        /// </summary>
        Double  Altitude       { get; set; }

    }

}

