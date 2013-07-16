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
using System.Windows.Controls;

using eu.Vanaheimr.Illias.Commons;
using eu.Vanaheimr.Aegir.Controls;
using System.Windows;

#endregion

namespace eu.Vanaheimr.Aegir
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
