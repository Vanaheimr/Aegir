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
using System.Windows.Controls;

using de.ahzf.Illias.Commons;
using de.ahzf.Vanaheimr.Aegir.Controls;
using System.Windows;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// The common interface for all map layers.
    /// </summary>
    public interface IMapLayer : IIdentifier<String>,
                                 IEquatable<IMapLayer>,
                                 IComparable<IMapLayer>,
                                 IComparable
    {

        #region Properties

        /// <summary>
        /// The hosting map control.
        /// </summary>
        MapControl MapControl { get; }

        /// <summary>
        /// The z-index of this feature layer.
        /// </summary>
        Int32 ZIndex { get; }

        /// <summary>
        /// Wether the map layer is currently visible or not.
        /// </summary>
        Boolean IsVisible { get; }

        /// <summary>
        /// The current visibility of the map layer.
        /// </summary>
        Visibility Visibility { get; set; }

        /// <summary>
        /// The features of this feature layer.
        /// </summary>
        UIElementCollection Children { get; }

        #endregion

        #region Interaction methods

        /// <summary>
        /// Set the zoom level and screen offset of this map layer.
        /// </summary>
        /// <param name="ZoomLevel">The desired zoom level.</param>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        IMapLayer ZoomTo(UInt32 ZoomLevel, Int64 ScreenOffsetX, Int64 ScreenOffsetY);

        /// <summary>
        /// Set the screen offset of this map layer.
        /// </summary>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        IMapLayer SetDisplayOffset(Int64 ScreenOffsetX, Int64 ScreenOffsetY);

        /// <summary>
        /// Redraws this feature layer.
        /// </summary>
        /// <returns>True if the layer was redrawn; false otherwise.</returns>
        Boolean Redraw();

        #endregion

    }

}
