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

using System.Windows.Input;
using System.Windows.Media;

using de.ahzf.Vanaheimr.Aegir.Controls;
using System.Windows.Shapes;
using System.Windows;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for adding, editing and visualizing map features.
    /// </summary>
    public class EditFeatureLayer : AMapLayer
    {

        #region Constructor(s)

        #region EditFeatureLayer(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)

        /// <summary>
        /// Creates a new edit feature layer for adding, editing and visualizing map features.
        /// </summary>
        /// <param name="Name">The identification string of this feature layer.</param>
        /// <param name="ZoomLevel">The the zoom level of this feature layer.</param>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        /// <param name="MapControl">The parent map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public EditFeatureLayer(String Id, UInt32 ZoomLevel, Int64 ScreenOffsetX, Int64 ScreenOffsetY, MapControl MapControl, Int32 ZIndex)
            : base(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)
        {

            this.Background = new SolidColorBrush(Colors.Transparent);

            #region Register mouse events

            this.PreviewMouseMove            += MapControl.ProcessMouseMove;
            this.MouseLeftButtonDown         += MapControl.ProcessMouseLeftButtonDown;
            this.PreviewMouseLeftButtonDown  += MapControl.ProcessMouseLeftDoubleClick;
            this.MouseWheel                  += MapControl.ProcessMouseWheel;

            this.PreviewMouseRightButtonDown += ProcessPreviewMouseRightButtonDown;

            #endregion

        }

        #endregion

        #endregion


        #region AddFeature


        public Feature AddFeature(String Id, Latitude Latitude, Longitude Longitude, Double Width, Double Height, Color Color)
        {
            return AddFeature(Id, new GeoCoordinate(Latitude, Longitude), Width, Height, Color);
        }

        public Feature AddFeature(String Id, GeoCoordinate GeoCoordinate, Double Width, Double Height, Color Color)
        {

            var Feature              = new Feature(new EllipseGeometry() { RadiusX = Width/2, RadiusY = Height/2 });
            Feature.Id               = Id;
            Feature.Latitude         = GeoCoordinate.Latitude;
            Feature.Longitude        = GeoCoordinate.Longitude;
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


        private void ProcessPreviewMouseRightButtonDown(Object Sender, MouseEventArgs MouseEventArgs)
        {
            
            var Mouse = MouseEventArgs.GetPosition(this);

            AddFeature("jfgdh", GeoCalculations.Mouse_2_WorldCoordinates(Mouse.X - this.ScreenOffsetX,
                                                                         Mouse.Y - this.ScreenOffsetY,
                                                                         this.ZoomLevel), 5, 5, Colors.Blue);

            Redraw();

        }


    }

}
