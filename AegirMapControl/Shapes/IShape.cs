﻿/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
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

using eu.Vanaheimr.Illias.Commons;
using System.Windows.Media;
using System.Windows;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// The common interface for all features on an Aegir map.
    /// </summary>
    public interface IShape : IIdentifier<String>,
                              IReadonlyGeoPosition,
                              IEquatable<IFeature>,
                              IComparable<IFeature>,
                              IComparable
    {

        /// <summary>
        /// The geographical width of something.
        /// </summary>
        Latitude  Latitude2  { get; }

        /// <summary>
        /// The geographical height of something.
        /// </summary>
        Longitude Longitude2 { get; }


        Rect Bounds { get; }

        UInt32 ZoomLevel { get; set; }

    }

}
