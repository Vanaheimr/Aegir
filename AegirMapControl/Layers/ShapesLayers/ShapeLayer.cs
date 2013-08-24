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
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;

using eu.Vanaheimr.Aegir.Controls;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing map features.
    /// </summary>
    public class ShapeLayer : AMapLayer
    {

        #region Constructor(s)

        #region ShapeLayer(Id, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        /// <param name="Name">The identification string of this feature layer.</param>
        /// <param name="MapControl">The parent map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public ShapeLayer(String Id, MapControl MapControl, Int32 ZIndex)
            : base(Id, MapControl, ZIndex)
        {

            #region Register mouse events

            this.PreviewMouseMove           += MapControl.ProcessMouseMove;
            this.MouseLeftButtonDown        += MapControl.ProcessMouseLeftButtonDown;
            this.PreviewMouseLeftButtonDown += MapControl.ProcessMouseLeftDoubleClick;
            this.MouseWheel                 += MapControl.ProcessMouseWheel;

            #endregion

            this.MapControl.ZoomLevelChanged += (s, o, n) => SetZoomLevel(n);

        }

        #endregion

        #endregion


        public GeoShape AddShape(GeoShape AShape)
        {

            // The position and size on the map will be set within the PaintMap() method!
            this.Children.Add(AShape);
            AShape.ZoomLevel = this.MapControl.ZoomLevel;

            return AShape;

        }

        private void SetZoomLevel(UInt32 ZoomLevel)
        {
            this.Children.
                ForEach<GeoShape>(AShape => AShape.ZoomLevel = ZoomLevel);
        }

        public Feature AddPath(String Id, Latitude Latitude, Longitude Longitude, Double Width, Double Height, Color Color)
        {

            var XY = GeoCalculations.WorldCoordinates_2_Screen(Latitude, Longitude, this.MapControl.ZoomLevel);

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

            var Feature = new Feature(Id, new RectangleGeometry() { Rect = DrawingGroup.Bounds }) {
                Latitude   = Latitude,
                Longitude  = Longitude,
                Width      = DrawingGroup.Bounds.Width,
                Height     = DrawingGroup.Bounds.Height,
                Fill       = DrawingBrush,
                ToolTip    = Id
            };

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

                    this.Children.
                         ForEach<GeoShape>(AShape => {

                            AShape.ZoomLevel = this.MapControl.ZoomLevel;

                            Canvas.SetLeft(AShape, this.MapControl.ScreenOffset.X + (Int64) AShape.OnScreenUpperLeft.X);
                            Canvas.SetTop (AShape, this.MapControl.ScreenOffset.Y + (Int64) AShape.OnScreenLowerRight.Y);

                            AShape.Width  = AShape.OnScreenWidth;
                            AShape.Height = AShape.OnScreenHeight;

                        });

                }

                IsCurrentlyPainting = false;

                return true;

            }

            return false;

        }

        #endregion


    }

}
