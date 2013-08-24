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
using System.Windows.Input;
using System.Windows.Media;

using eu.Vanaheimr.Aegir.Controls;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for adding, editing and visualizing map features.
    /// </summary>
    public class EditFeatureLayer : FeatureLayer
    {

        #region Constructor(s)

        #region EditFeatureLayer(Id, MapControl, ZIndex)

        /// <summary>
        /// Creates a new edit feature layer for adding, editing and visualizing map features.
        /// </summary>
        /// <param name="Name">The identification string of this feature layer.</param>
        /// <param name="MapControl">The parent map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public EditFeatureLayer(String Id, MapControl MapControl, Int32 ZIndex)
            : base(Id, MapControl, ZIndex)
        {

            this.Background = new SolidColorBrush(Colors.Transparent);

            // Register mouse events
            this.PreviewMouseRightButtonDown += ProcessPreviewMouseRightButtonDown;

        }

        #endregion

        #endregion


        private void ProcessPreviewMouseRightButtonDown(Object Sender, MouseEventArgs MouseEventArgs)
        {
            
            var Mouse = MouseEventArgs.GetPosition(this);

            AddFeature("NewFeature",
                       GeoCalculations.Mouse_2_WorldCoordinates(Mouse.X - this.MapControl.ScreenOffset.X,
                                                                Mouse.Y - this.MapControl.ScreenOffset.Y,
                                                                this.MapControl.ZoomLevel),
                                                                5, 5,
                                                                Brushes.Blue,
                                                                Brushes.Black,
                                                                1.0);

            Redraw();

        }


    }

}
