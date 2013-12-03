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
using System.Linq;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

using eu.Vanaheimr.Illias.Geometry;
using eu.Vanaheimr.Aegir.Controls;
using System.Windows.Shapes;

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
        private List<TriCircle> DelaunayTriangleList;

        #endregion

        #region Properties

        public Boolean Delaunay         { get; set; }
        public Boolean Voronoi          { get; set; }
        public Boolean VoronoiOutback   { get; set; }
        public Boolean CircumCenters    { get; set; }
        public Boolean DrawCircumCircles    { get; set; }

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
        public VoronoiFeature AddFeature(String     Id,
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
        public VoronoiFeature AddFeature(String Id,
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

                if (DelaunayTriangleList == null)
                    DelaunayTriangleList = VoronoiFeatures.DelaunayTriangulation().ToList();

                if (DelaunayTriangleList != null)
                    foreach (var DelaunayTriangle in DelaunayTriangleList) {

                        var P1OnScreen = GeoCalculations.GeoCoordinate2ScreenXY(DelaunayTriangle.Triangle.P1, this.MapControl.ZoomLevel);
                        var P2OnScreen = GeoCalculations.GeoCoordinate2ScreenXY(DelaunayTriangle.Triangle.P2, this.MapControl.ZoomLevel);
                        var P3OnScreen = GeoCalculations.GeoCoordinate2ScreenXY(DelaunayTriangle.Triangle.P3, this.MapControl.ZoomLevel);

                        ////DrawLine(P1OnScreen, P2OnScreen, Brushes.Blue, 1.0);
                        ////DrawLine(P2OnScreen, P3OnScreen, Brushes.Blue, 1.0);
                        ////DrawLine(P3OnScreen, P1OnScreen, Brushes.Blue, 1.0);

                        //var GeoCircle      = new GeoCircle(_Triangle.P1, _Triangle.P2, _Triangle.P3);
                        var ScreenCircle = new Circle<Double>(new Pixel<Double>(P1OnScreen.X + this.MapControl.ScreenOffset.X, P1OnScreen.Y + this.MapControl.ScreenOffset.Y),
                                                              new Pixel<Double>(P2OnScreen.X + this.MapControl.ScreenOffset.X, P2OnScreen.Y + this.MapControl.ScreenOffset.Y),
                                                              new Pixel<Double>(P3OnScreen.X + this.MapControl.ScreenOffset.X, P3OnScreen.Y + this.MapControl.ScreenOffset.Y));

                        //var r1 = ScreenCircle.Center.DistanceTo(P1OnScreen.X + this.MapControl.ScreenOffset.X, P1OnScreen.Y + this.MapControl.ScreenOffset.Y);
                        //var r2 = ScreenCircle.Center.DistanceTo(P2OnScreen.X + this.MapControl.ScreenOffset.X, P2OnScreen.Y + this.MapControl.ScreenOffset.Y);
                        //var r3 = ScreenCircle.Center.DistanceTo(P3OnScreen.X + this.MapControl.ScreenOffset.X, P3OnScreen.Y + this.MapControl.ScreenOffset.Y);

                        //var CenterOnScreen = GeoCalculations.GeoCoordinate2ScreenXY(GeoCircle.Center, this.MapControl.ZoomLevel);

                        #region DrawCircumCircles

                        if (DrawCircumCircles)
                        {

                            var CircumCircle = new Ellipse() {
                                Width           = ScreenCircle.Diameter,
                                Height          = ScreenCircle.Diameter,
                                Stroke          = Brushes.DarkGreen,
                                StrokeThickness = 1.0
                            };

                            this.Children.Add(CircumCircle);
                            //Canvas.SetLeft(CircumCircle, CenterOnScreen.X + this.MapControl.ScreenOffset.X);
                            //Canvas.SetTop (CircumCircle, CenterOnScreen.Y + this.MapControl.ScreenOffset.Y);
                            Canvas.SetLeft(CircumCircle, ScreenCircle.X - ScreenCircle.Radius);// + this.MapControl.ScreenOffset.X);
                            Canvas.SetTop (CircumCircle, ScreenCircle.Y - ScreenCircle.Radius);// + this.MapControl.ScreenOffset.Y);

                        }

                        #endregion

                    }

                foreach (var tr1 in DelaunayTriangleList)
                    tr1.Neighbors.Clear();

                HashSet<GeoCoordinate> tr1_h, tr2_h;
                IEnumerable<GeoCoordinate> Intersection;

                foreach (var tr1 in DelaunayTriangleList)
                {
                    foreach (var tr2 in DelaunayTriangleList)
                    {

                        tr1_h = new HashSet<GeoCoordinate>();
                        tr1_h.Add(tr1.Triangle.P1);
                        tr1_h.Add(tr1.Triangle.P2);
                        tr1_h.Add(tr1.Triangle.P3);

                        tr2_h = new HashSet<GeoCoordinate>();
                        tr2_h.Add(tr2.Triangle.P1);
                        tr2_h.Add(tr2.Triangle.P2);
                        tr2_h.Add(tr2.Triangle.P3);

                        Intersection = tr1_h.Intersect(tr2_h);

                        if (Intersection.Count() == 2)
                        {

                            tr1.Neighbors.Add(tr2);
                            tr2.Neighbors.Add(tr1);

                            foreach (var bo in tr1.Triangle.Borders)
                                if (Intersection.Contains(bo.P1) && Intersection.Contains(bo.P2))
                                    bo.Tags.Add("shared");

                            foreach (var bo in tr2.Triangle.Borders)
                                if (Intersection.Contains(bo.P1) && Intersection.Contains(bo.P2))
                                    bo.Tags.Add("shared");

                        }

                    }
                }

                var aaa = DelaunayTriangleList.SelectMany(v => v.Triangle.Borders).Select(v => v.Tags);

                foreach (var DelaunayTriangle in DelaunayTriangleList)
                    foreach (var Edge in DelaunayTriangle.Triangle.Borders)
                {

                    DrawLine(GeoCalculations.GeoCoordinate2ScreenXY(Edge.P1, this.MapControl.ZoomLevel),
                             GeoCalculations.GeoCoordinate2ScreenXY(Edge.P2, this.MapControl.ZoomLevel),
                             (Edge.Tags.Contains("shared"))
                                ? Brushes.LightBlue
                                : Brushes.Blue,
                             1.0);

                }

                foreach (var tr1 in DelaunayTriangleList)//.Skip(2).Take(1))
                {

                    var Borders = tr1.Triangle.Borders.ToList();

                    var Center = new GeoLine(Borders[0].Center, Borders[0].Normale).
                                     Intersection(new GeoLine(Borders[1].Center, Borders[1].Normale));

                    var Vector1  = Borders[0].Vector;
                    var Vector2  = Borders[0].Vector;
                    var Vector3  = Borders[0].Vector;

                    var Normale1 = Borders[0].Normale;
                    var Normale2 = Borders[0].Normale;
                    var Normale3 = Borders[0].Normale;

                    //var Center1  = new GeoCoordinate(new Latitude(Borders[0].Center.Longitude.Value), new Longitude(Borders[0].Center.Latitude.Value));
                    //var Center2  = new GeoCoordinate(new Latitude(Borders[1].Center.Longitude.Value), new Longitude(Borders[1].Center.Latitude.Value));
                    //var Center3  = new GeoCoordinate(new Latitude(Borders[2].Center.Longitude.Value), new Longitude(Borders[2].Center.Latitude.Value));

                    DrawLine(Center, Borders[0].Center, Brushes.Green, 1.0);
                    DrawLine(Center, Borders[1].Center, Brushes.Green, 1.0);
                    DrawLine(Center, Borders[2].Center, Brushes.Green, 1.0);

                }

                #endregion


                //var Center1                 = new GeoCoordinate(new Latitude(49.7316155727453), new Longitude(10.1409612894059));

                //var LineX_3_4               = new GeoLine(new GeoCoordinate(new Latitude(49.732745964269350), new Longitude(10.135724544525146)),
                //                                          new GeoCoordinate(new Latitude(49.731761237693235), new Longitude(10.135746002197264)));

                //var Line_S3S4               = new GeoLine(new GeoCoordinate(new Latitude(49.732552), new Longitude(10.139216)),
                //                                          new GeoCoordinate(new Latitude(49.731004), new Longitude(10.138913)));

                //var Line_S3S4Center2Center1 = new GeoLine(Line_S3S4.Center,
                //                                          Center1);

                //var Intersection1           = Line_S3S4Center2Center1.Intersection(LineX_3_4);

                //DrawLine(Line_S3S4.Center, Intersection1, Brushes.Red, 1.0);

                //// ------------------------------------------------------------------------

                //var LineX_7_8               = new GeoLine(new GeoCoordinate(new Latitude(49.729930425324014), new Longitude(10.137097835540771)),
                //                                          new GeoCoordinate(new Latitude(49.729347879633465), new Longitude(10.138492584228516)));

                //var Line_S4S5               = new GeoLine(new GeoCoordinate(new Latitude(49.731004), new Longitude(10.138913)),
                //                                          new GeoCoordinate(new Latitude(49.730237), new Longitude(10.140107)));

                //var Line_S4S5Center2Center1 = new GeoLine(Line_S4S5.Center,
                //                                          Center1);

                //var Intersection2           = Line_S4S5Center2Center1.Intersection(LineX_7_8);

                //DrawLine(Line_S4S5.Center, Intersection2, Brushes.Red, 1.0);

                //// ------------------------------------------------------------------------

                //var Center2                 = new GeoCoordinate(new Latitude(49.7302216912131), new Longitude(10.1434879302979));

                //var LineX_14_15             = new GeoLine(new GeoCoordinate(new Latitude(49.728695974976030), new Longitude(10.143170356750488)),
                //                                          new GeoCoordinate(new Latitude(49.728987252607084), new Longitude(10.144414901733398)));

                //var Line_S5S7               = new GeoLine(new GeoCoordinate(new Latitude(49.730237), new Longitude(10.140107)),
                //                                          new GeoCoordinate(new Latitude(49.730664), new Longitude(10.146802)));

                //var Line_S5S7Center2Center2 = new GeoLine(Line_S5S7.Center,
                //                                          Center2);

                //var Intersection3           = Line_S5S7Center2Center2.Intersection(LineX_14_15);

                //DrawLine(Center2, Intersection3, Brushes.Red, 1.0);

                //// ------------------------------------------------------------------------

                //var LineX_17_18             = new GeoLine(new GeoCoordinate(new Latitude(49.732413101183180), new Longitude(10.149564743041992)),
                //                                          new GeoCoordinate(new Latitude(49.731469976708340), new Longitude(10.148684978485107)));

                //var Line_S7S6               = new GeoLine(new GeoCoordinate(new Latitude(49.730664), new Longitude(10.146802)),
                //                                          new GeoCoordinate(new Latitude(49.731791), new Longitude(10.145903)));

                //var Line_S7S6Center2Center2 = new GeoLine(Line_S7S6.Center,
                //                                          Center2);

                //var Intersection4           = Line_S7S6Center2Center2.Intersection(LineX_17_18);

                //DrawLine(Line_S7S6.Center, Intersection4, Brushes.Red, 1.0);

                //// ------------------------------------------------------------------------

                //var Center3                 = new GeoCoordinate(new Latitude(49.7318726185525), new Longitude(10.1424804925919));

                //var LineX_23_24             = new GeoLine(new GeoCoordinate(new Latitude(49.734146738072006), new Longitude(10.144844055175781)),
                //                                          new GeoCoordinate(new Latitude(49.733966442720840), new Longitude(10.142934322357178)));

                //var Line_S6S3               = new GeoLine(new GeoCoordinate(new Latitude(49.731791), new Longitude(10.145903)),
                //                                          new GeoCoordinate(new Latitude(49.732552), new Longitude(10.139216)));

                //var Line_S6S3Center2Center3 = new GeoLine(Line_S6S3.Center,
                //                                          Center3);

                //var Intersection5           = Line_S6S3Center2Center3.Intersection(LineX_23_24);

                //DrawLine(Line_S6S3.Center, Intersection5, Brushes.Red, 1.0);

            }

        }


        private void DrawLine(ScreenXY P1, ScreenXY P2, Brush Stroke, Double StrokeThickness)
        {

            var Line = new Line() {
                X1               = P1.X + this.MapControl.ScreenOffset.X,
                Y1               = P1.Y + this.MapControl.ScreenOffset.Y,
                X2               = P2.X + this.MapControl.ScreenOffset.X,
                Y2               = P2.Y + this.MapControl.ScreenOffset.Y,
                Stroke           = Stroke,
                StrokeThickness  = StrokeThickness
            };

            this.Children.Add(Line);

        }

        private void DrawLine(GeoLine Line, Brush Stroke, Double StrokeThickness)
        {
            DrawLine(Line.P1, Line.P2, Stroke, StrokeThickness);
        }

        private void DrawLine(GeoCoordinate P1, GeoCoordinate P2, Brush Stroke, Double StrokeThickness)
        {

            var Line = new Line()
            {
                X1 = GeoCalculations.GeoCoordinate2ScreenXY(P1, this.MapControl.ZoomLevel).X + this.MapControl.ScreenOffset.X,
                Y1 = GeoCalculations.GeoCoordinate2ScreenXY(P1, this.MapControl.ZoomLevel).Y + this.MapControl.ScreenOffset.Y,
                X2 = GeoCalculations.GeoCoordinate2ScreenXY(P2, this.MapControl.ZoomLevel).X + this.MapControl.ScreenOffset.X,
                Y2 = GeoCalculations.GeoCoordinate2ScreenXY(P2, this.MapControl.ZoomLevel).Y + this.MapControl.ScreenOffset.Y,
                Stroke = Stroke,
                StrokeThickness = StrokeThickness
            };

            this.Children.Add(Line);

        }

        private void DrawCircle(ScreenXY P, Double Radius, Brush Stroke, Double StrokeThickness)
        {

            var Ellipse = new Ellipse()
            {
                Width  = 2*Radius,
                Height = 2*Radius,
                Stroke = Stroke,
                StrokeThickness = StrokeThickness
            };

            this.Children.Add(Ellipse);

            Canvas.SetLeft(Ellipse, P.X - Radius + this.MapControl.ScreenOffset.X);
            Canvas.SetTop (Ellipse, P.Y - Radius + this.MapControl.ScreenOffset.Y);

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

                    this.Children.Clear();
                    Calc();

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
