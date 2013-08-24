﻿/*
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

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing map features.
    /// </summary>
    public class FeatureLayer : AMapLayer
    {

        #region Constructor(s)

        #region FeatureLayer(Id, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        /// <param name="Name">The identification string of this feature layer.</param>
        /// <param name="MapControl">The parent map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public FeatureLayer(String Id, MapControl MapControl, Int32 ZIndex)
            : base(Id, MapControl, ZIndex)
        {

            // Register mouse events
            this.PreviewMouseMove           += MapControl.ProcessMouseMove;
            this.MouseLeftButtonDown        += MapControl.ProcessMouseLeftButtonDown;
            this.PreviewMouseLeftButtonDown += MapControl.ProcessMouseLeftDoubleClick;
            this.MouseWheel                 += MapControl.ProcessMouseWheel;

        }

        #endregion

        #endregion


        #region AddFeature(Id, Latitude, Longitude, Width, Height, Fill, Stroke, StrokeThickness)

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
                                  Brush      Fill,
                                  Brush      Stroke,
                                  Double     StrokeThickness)

        {

            return AddFeature(Id,
                              new GeoCoordinate(Latitude, Longitude),
                              Width, Height,
                              Fill, Stroke, StrokeThickness);

        }

        #endregion

        #region AddFeature(Id, Latitude, Longitude, Altitude, Width, Height, Fill, Stroke, StrokeThickness)

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
                                  Brush      Fill,
                                  Brush      Stroke,
                                  Double     StrokeThickness)

        {

            return AddFeature(Id,
                              new GeoCoordinate(Latitude, Longitude, Altitude),
                              Width, Height,
                              Fill, Stroke, StrokeThickness);

        }

        #endregion

        #region AddFeature(Id, GeoCoordinate, Width, Height, Fill, Stroke, StrokeThickness)

        /// <summary>
        /// Add a map feature of the given dimensions at the given geo coordinate.
        /// </summary>
        /// <param name="Id">The unique identification of the feature.</param>
        /// <param name="GeoCoordinate">The geo coordinate of the feature.</param>
        /// <param name="Width">The longitude of the feature.</param>
        /// <param name="Height">The longitude of the feature.</param>
        /// <param name="Fill">The fill brush of the feature.</param>
        /// <param name="Stroke">The stroke brush of the feature.</param>
        /// <param name="StrokeThickness">The thickness of the stroke.</param>
        public Feature AddFeature(String         Id,
                                  GeoCoordinate  GeoCoordinate,
                                  Double         Width,
                                  Double         Height,
                                  Brush          Fill,
                                  Brush          Stroke,
                                  Double         StrokeThickness)
        {

            var Feature = new Feature(Id, new EllipseGeometry() { RadiusX = Width/2, RadiusY = Height/2 }) {
                Latitude         = GeoCoordinate.Latitude,
                Longitude        = GeoCoordinate.Longitude,
                Altitude         = GeoCoordinate.Altitude,
                Width            = Width,
                Height           = Height,
                Fill             = Fill,
                Stroke           = Stroke,
                StrokeThickness  = StrokeThickness,
                ToolTip          = Id
            };

            // The position on the map will be set within the PaintMap() method!
            this.Children.Add(Feature);

            return Feature;

        }

        #endregion

    }

}
