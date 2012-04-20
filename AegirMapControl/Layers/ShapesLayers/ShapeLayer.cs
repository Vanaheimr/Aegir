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

using System;

using System.Windows.Input;
using System.Windows.Media;

using de.ahzf.Vanaheimr.Aegir.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing map features.
    /// </summary>
    public class ShapeLayer : AMapLayer
    {

        #region Constructor(s)

        #region ShapeLayer(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        /// <param name="Name">The identification string of this feature layer.</param>
        /// <param name="ZoomLevel">The the zoom level of this feature layer.</param>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        /// <param name="MapControl">The parent map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public ShapeLayer(String Id, UInt32 ZoomLevel, Int64 ScreenOffsetX, Int64 ScreenOffsetY, MapControl MapControl, Int32 ZIndex)
            : base(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)
        {

            #region Register mouse events

            this.PreviewMouseMove           += MapControl.ProcessMouseMove;
            this.MouseLeftButtonDown        += MapControl.ProcessMouseLeftButtonDown;
            this.PreviewMouseLeftButtonDown += MapControl.ProcessMouseLeftDoubleClick;
            this.MouseWheel                 += MapControl.ProcessMouseWheel;

            #endregion

        }

        #endregion

        #endregion


        public IShape AddShape(AShape AShape)
        {

            AShape.ToolTip = AShape.GetType().Name;

            // The position and size on the map will be set within the PaintMap() method!
            this.Children.Add(AShape);

            return AShape;

        }



        public Feature AddPath(String Id, Double Latitude, Double Longitude, Double Width, Double Height, Color Color)
        {

            var XY = GeoCalculations.WorldCoordinates_2_Screen(Latitude, Longitude, ZoomLevel);

            var PathGeometry1 = PathGeometry.Parse("M51,42c-5-4-11-7-19-7c-6,0-12,1-20,5l10-35c20-8,30-4,39,2l-10,35z");
            var PathGeometry2 = PathGeometry.Parse("M106,13c-21,9-31,4-40-2l-10,35c9,6,20,11,40,2l10-35z");
            var PathGeometry3 = PathGeometry.Parse("M39,83c-9-6-18-10-39-2l10-35c21-9,31-4,39,2l-10,35z");
            var PathGeometry4 = PathGeometry.Parse("M55,52c9,6,18,10,39,2l-10,35c-21,8-30,3-39-3l10-34z");

            var pathText = PathGeometry1.ToString();

            var GD1 = new GeometryDrawing(new SolidColorBrush(Color.FromRgb(0xE0, 0x60, 0x30)), null, PathGeometry1);
            var GD2 = new GeometryDrawing(new SolidColorBrush(Color.FromRgb(0x70, 0xB0, 0x40)), null, PathGeometry2);
            var GD3 = new GeometryDrawing(new SolidColorBrush(Color.FromRgb(0x40, 0x90, 0xc0)), null, PathGeometry3);
            var GD4 = new GeometryDrawing(new SolidColorBrush(Color.FromRgb(0xF0, 0xD0, 0x50)), null, PathGeometry4);

            var DrawingGroup = new DrawingGroup();
            DrawingGroup.Children.Add(GD1);
            DrawingGroup.Children.Add(GD2);
            DrawingGroup.Children.Add(GD3);
            DrawingGroup.Children.Add(GD4);

            var DrawingBrush = new DrawingBrush() {
                Drawing  = DrawingGroup,
                TileMode = TileMode.None
            };

            var Feature       = new Feature(new RectangleGeometry() { Rect = DrawingGroup.Bounds });
            Feature.Id        = Id;
            Feature.Latitude  = Latitude;
            Feature.Longitude = Longitude;
            Feature.Width     = DrawingGroup.Bounds.Width;
            Feature.Height    = DrawingGroup.Bounds.Height;
            Feature.Fill      = DrawingBrush;
            Feature.ToolTip   = Id;

            // The position on the map will be set within the PaintMap() method!
            this.Children.Add(Feature);

            return Feature;

        }


        #region (override) Redraw()

        /// <summary>
        /// Redraws this feature layer.
        /// </summary>
        /// <returns>True if the map was repainted; false otherwise.</returns>
        public override Boolean Redraw()
        {

            if (this.IsVisible && !IsCurrentlyPainting)
            {

                IsCurrentlyPainting = true;

                if (!DesignerProperties.GetIsInDesignMode(this))
                {

                    AShape AShape;
                    Tuple<UInt64, UInt64> XY, WH;

                    foreach (var Child in this.Children)
                    {

                        AShape = Child as AShape;

                        if (AShape != null)
                        {

                            XY = GeoCalculations.WorldCoordinates_2_Screen(AShape.Latitude, AShape.Longitude, ZoomLevel);
                            Canvas.SetLeft(AShape, ScreenOffsetX + (Int64) XY.Item1);
                            Canvas.SetTop (AShape, ScreenOffsetY + (Int64) XY.Item2);

                            AShape.ZoomLevel = ZoomLevel;
                            WH = GeoCalculations.WorldCoordinates_2_Screen(47.270203, 15.041656, ZoomLevel);
                            AShape.Width  = WH.Item1 - XY.Item1;
                            AShape.Height = WH.Item2 - XY.Item2;

//                            AShape.DrawingBrush.Stretch = Stretch.UniformToFill;

                        }

                    }

                }

                IsCurrentlyPainting = false;

                return true;

            }

            return false;

        }

        #endregion


    }

}
