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
using System.Collections.Generic;
using System.Threading.Tasks;

using eu.Vanaheimr.Illias.Geometry;
using eu.Vanaheimr.Aegir.Controls;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing map features.
    /// </summary>
    public class VoronoiLayer : AMapLayer
    {

        #region Data

        private readonly List<VoronoiFeature> VoronoiFeatures;
        private List<ITriangle<Double>> TriangleList;

        #endregion

        #region Properties

        #region Delaunay
        #endregion

        #endregion

        #region Constructor(s)

        #region VoronoiLayer(Id, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        /// <param name="Name">The identification string of this feature layer.</param>
        /// <param name="MapControl">The parent map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public VoronoiLayer(String Id, MapControl MapControl, Int32 ZIndex)
            : base(Id, MapControl, ZIndex)
        {
            this.VoronoiFeatures = new List<VoronoiFeature>();
        }

        #endregion

        #endregion


        #region AddFeature(Id, Latitude, Longitude, Fill, Stroke, StrokeThickness)

        /// <summary>
        /// Add a map feature of the given dimensions at the given geo coordinate.
        /// </summary>
        /// <param name="Id">The unique identification of the feature.</param>
        /// <param name="Latitude">The latitude of the feature.</param>
        /// <param name="Longitude">The longitude of the feature.</param>
        /// <param name="Fill">The fill brush of the feature.</param>
        /// <param name="Stroke">The stroke brush of the feature.</param>
        /// <param name="StrokeThickness">The thickness of the stroke.</param>
        public Feature AddFeature(String     Id,
                                  Latitude   Latitude,
                                  Longitude  Longitude,
                                  Brush      Fill,
                                  Brush      Stroke,
                                  Double     StrokeThickness)

        {

            return AddFeature(Id,
                              new GeoCoordinate(Latitude, Longitude),
                              Fill,
                              Stroke, StrokeThickness);

        }

        #endregion

        #region AddFeature(Id, Latitude, Longitude, Altitude, Fill, Stroke, StrokeThickness)

        /// <summary>
        /// Add a map feature of the given dimensions at the given geo coordinate.
        /// </summary>
        /// <param name="Id">The unique identification of the feature.</param>
        /// <param name="Latitude">The latitude of the feature.</param>
        /// <param name="Longitude">The longitude of the feature.</param>
        /// <param name="Altitude">The altitude of the feature.</param>
        /// <param name="Fill">The fill brush of the feature.</param>
        /// <param name="Stroke">The stroke brush of the feature.</param>
        /// <param name="StrokeThickness">The thickness of the stroke.</param>
        public Feature AddFeature(String Id,
                                  Latitude   Latitude,
                                  Longitude  Longitude,
                                  Altitude   Altitude,
                                  Brush      Fill,
                                  Brush      Stroke,
                                  Double     StrokeThickness)

        {

            return AddFeature(Id,
                              new GeoCoordinate(Latitude, Longitude, Altitude),
                              Fill,
                              Stroke, StrokeThickness);

        }

        #endregion

        #region AddFeature(Id, GeoCoordinate, Width, Height, Fill, Stroke, StrokeThickness)

        /// <summary>
        /// Add a map feature of the given dimensions at the given geo coordinate.
        /// </summary>
        /// <param name="Id">The unique identification of the feature.</param>
        /// <param name="GeoCoordinate">The geo coordinate of the feature.</param>
        /// <param name="Fill">The fill brush of the feature.</param>
        /// <param name="Stroke">The stroke brush of the feature.</param>
        /// <param name="StrokeThickness">The thickness of the stroke.</param>
        public VoronoiFeature AddFeature(String         Id,
                                         GeoCoordinate  GeoCoordinate,
                                         Brush          Fill,
                                         Brush          Stroke,
                                         Double         StrokeThickness)
        {

            var Feature = new VoronoiFeature(Id, GeoCoordinate) {
                Fill             = Fill,
                Stroke           = Stroke,
                StrokeThickness  = StrokeThickness
            };

            this.VoronoiFeatures.Add(Feature);
            Calc();

            // The position on the map will be set within the PaintMap() method!
            this.Children.Add(Feature);

            return Feature;

        }

        #endregion


        private void Calc()
        {

            if (VoronoiFeatures.Count > 2)
            {

                #region Draw Delaunay

                //if (DelaunayCheckBox.IsChecked == true)
                //{

                    //Task.Factory.StartNew(() =>
                    //{

                        if (TriangleList == null)
                            TriangleList = VoronoiFeatures.DelaunayTriangulation().ToList();

                        foreach (var _Triangle in TriangleList)
                        {
                            DrawLine(_Triangle.P1.X, _Triangle.P1.Y, _Triangle.P2.X, _Triangle.P2.Y, Colors.Blue);
                            DrawLine(_Triangle.P2.X, _Triangle.P2.Y, _Triangle.P3.X, _Triangle.P3.Y, Colors.Blue);
                            DrawLine(_Triangle.P3.X, _Triangle.P3.Y, _Triangle.P1.X, _Triangle.P1.Y, Colors.Blue);
                        }

                    //}, _CancellationToken, TaskCreationOptions.None, _TaskSchedulerContext);

                //}

                #endregion


            }

        }


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
