﻿/*
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

using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Concurrent;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using de.Vanaheimr.Aegir.Tiles;
using de.ahzf.Vanaheimr.Aegir;
using System.Windows.Shapes;
using de.Vanaheimr.Aegir.Controls;

#endregion

namespace de.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing map features.
    /// </summary>
    public class FeatureLayer : AFeatureLayer
    {

        #region Constructor(s)

        #region FeatureLayer(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        /// <param name="Name">The identification string of this feature layer.</param>
        /// <param name="ZoomLevel">The the zoom level of this feature layer.</param>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        /// <param name="MapControl">The parent map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public FeatureLayer(String Id, UInt32 ZoomLevel, Int32 ScreenOffsetX, Int32 ScreenOffsetY, MapControl MapControl, Int32 ZIndex)
            : base(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)
        { }

        #endregion

        #endregion


        #region ProcessMouseLeftButtonDown

        public void ProcessMouseLeftButtonDown(Object Sender, MouseButtonEventArgs MouseButtonEventArgs)
        {

            // We'll need this for when the Form starts to move
            var MousePosition = MouseButtonEventArgs.GetPosition(this);
            LastClickPositionX = MousePosition.X;
            LastClickPositionY = MousePosition.Y;

            DrawingOffset_AtMovementStart_X = ScreenOffsetX;
            DrawingOffset_AtMovementStart_Y = ScreenOffsetY;

            MouseButtonEventArgs.Handled = false;

        }

        #endregion


        #region AddFeature

        public override Feature AddFeature(String Id, Double Latitude, Double Longitude, Double Width, Double Height, Color Color)
        {

            var XY = GeoCalculations.WorldCoordinates_2_Screen(Latitude, Longitude, (Int32) ZoomLevel);

            var Feature              = new Feature(new EllipseGeometry() { RadiusX = Width/2, RadiusY = Height/2 });
            Feature.Id               = Id;
            Feature.Latitude         = Latitude;
            Feature.Longitude        = Longitude;
            Feature.Stroke           = new SolidColorBrush(Colors.Black);
            Feature.StrokeThickness  = 1;
            Feature.Width            = Width;
            Feature.Height           = Height;
            Feature.Fill             = new SolidColorBrush(Color);
            Feature.ToolTip          = Id;

            // The position on the map will be set within the PaintMap() method!
            this.Children.Add(Feature);
            
            return Feature;

        }

        #endregion

    }

}
