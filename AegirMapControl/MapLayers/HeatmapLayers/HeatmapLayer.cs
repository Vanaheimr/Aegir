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

using eu.Vanaheimr.Aegir.Controls;
using System.ComponentModel;
using System.Windows.Controls;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing a heatmap.
    /// </summary>
    public class HeatmapLayer : AMapLayer
    {

        #region Constructor(s)

        #region HeatmapLayer(Id, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing a heatmap.
        /// </summary>
        /// <param name="Name">The identification string of this feature layer.</param>
        /// <param name="MapControl">The parent map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public HeatmapLayer(String Id, MapControl MapControl, Int32 ZIndex)
            : base(Id, MapControl, ZIndex)
        {
            this.IsHitTestVisible = false;
        }

        #endregion

        #endregion


        #region AddFeature(Id, Latitude, Longitude, Width, Height, Color)

        /// <summary>
        /// Add a map feature of the given dimensions at the given geo coordinate.
        /// </summary>
        /// <param name="Id">The unique identification of the feature.</param>
        /// <param name="Latitude">The latitude of the feature.</param>
        /// <param name="Longitude">The longitude of the feature.</param>
        /// <param name="Width">The longitude of the feature.</param>
        /// <param name="Height">The longitude of the feature.</param>
        /// <param name="Fill">The fill brush of the feature.</param>
        /// <param name="Stroke">The stroke brush of the feature.</param>
        /// <param name="StrokeThickness">The thickness of the stroke.</param>
        public Feature AddFeature(String     Id,
                                  Latitude   Latitude,
                                  Longitude  Longitude,
                                  Double     Width,
                                  Double     Height,
                                  Color      Color)

        {

            return AddFeature(Id,
                              new GeoCoordinate(Latitude, Longitude),
                              Width, Height,
                              Color);

        }

        #endregion

        #region AddFeature(Id, Latitude, Longitude, Altitude, Width, Height, Color)

        /// <summary>
        /// Add a map feature of the given dimensions at the given geo coordinate.
        /// </summary>
        /// <param name="Id">The unique identification of the feature.</param>
        /// <param name="Latitude">The latitude of the feature.</param>
        /// <param name="Longitude">The longitude of the feature.</param>
        /// <param name="Altitude">The altitude of the feature.</param>
        /// <param name="Width">The longitude of the feature.</param>
        /// <param name="Height">The longitude of the feature.</param>
        /// <param name="Fill">The fill brush of the feature.</param>
        /// <param name="Stroke">The stroke brush of the feature.</param>
        /// <param name="StrokeThickness">The thickness of the stroke.</param>
        public Feature AddFeature(String Id,
                                  Latitude   Latitude,
                                  Longitude  Longitude,
                                  Altitude   Altitude,
                                  Double     Width,
                                  Double     Height,
                                  Color      Color)

        {

            return AddFeature(Id,
                              new GeoCoordinate(Latitude, Longitude, Altitude),
                              Width, Height,
                              Color);

        }

        #endregion

        #region AddFeature(Id, GeoCoordinate, Width, Height, Color)

        /// <summary>
        /// Add a map feature of the given dimensions at the given geo coordinate.
        /// </summary>
        /// <param name="Id">The unique identification of the feature.</param>
        /// <param name="GeoCoordinate">The geo coordinate of the feature.</param>
        /// <param name="Width">The longitude of the feature.</param>
        /// <param name="Height">The longitude of the feature.</param>
        /// <param name="Color">The fill color of the feature.</param>
        public Feature AddFeature(String         Id,
                                  GeoCoordinate  GeoCoordinate,
                                  Double         Width,
                                  Double         Height,
                                  Color          Color)
        {

            var radialBrush = new RadialGradientBrush();
            var ColorHigh   = Color; ColorHigh.A = 0xFF;
            var ColorLow    = Color; ColorLow.A  = 0x00;

            radialBrush.GradientStops.Add(new GradientStop(ColorHigh, 0.0));
            radialBrush.GradientStops.Add(new GradientStop(ColorLow,  1.0));

            var XY = GeoCalculations.GeoCoordinate2ScreenXY(GeoCoordinate, this.MapControl.ZoomLevel);


            var Feature = new Feature(Id, new EllipseGeometry() { RadiusX = Width/2, RadiusY = Height/2 }) {
                Latitude         = GeoCoordinate.Latitude,
                Longitude        = GeoCoordinate.Longitude,
                Altitude         = GeoCoordinate.Altitude,
                Width            = Width,
                Height           = Height,
                Fill             = radialBrush,
                ToolTip          = Id
            };

            // The position on the map will be set within the PaintMap() method!
            this.Children.Add(Feature);

            return Feature;

        }

        #endregion


        #region (override) Move(X, Y)

        /// <summary>
        /// Move all the shapes on this mapping layer.
        /// </summary>
        /// <param name="X">X</param>
        /// <param name="Y">Y</param>
        public override void Move(Double X, Double Y)
        {
            Redraw();
        }

        #endregion

        #region (override) Redraw()

        /// <summary>
        /// Redraws this mapping layer.
        /// </summary>
        public override void Redraw()
        {

            if (this.IsVisible && !IsCurrentlyPainting)
            {

                IsCurrentlyPainting = true;

                if (!DesignerProperties.GetIsInDesignMode(this))
                {

                    this.Children.
                         ForEach<Feature>(AFeature => {

                             var ScreenXY = GeoCalculations.GeoCoordinate2ScreenXY(AFeature.Latitude,
                                                                                   AFeature.Longitude,
                                                                                   MapControl.ZoomLevel);

                             Canvas.SetLeft(AFeature, this.MapControl.ScreenOffset.X + ScreenXY.X);
                             Canvas.SetTop (AFeature, this.MapControl.ScreenOffset.Y + ScreenXY.Y);

                         });

                }

                IsCurrentlyPainting = false;

            }

        }

        #endregion


    }

}
