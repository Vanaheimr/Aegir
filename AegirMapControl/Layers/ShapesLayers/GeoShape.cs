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
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// A polygon shape on an Aegir map.
    /// </summary>
    public class GeoShape : Shape,
                            IEquatable<GeoShape>, IComparable<GeoShape>,
                            IEquatable<String>,   IComparable<String>
    {

        #region Data

        private IEnumerable<GeoCoordinate> GeoCoordinates;

        #endregion

        #region Properties

        #region Id

        /// <summary>
        /// The identification of this shape.
        /// </summary>
        public String    Id         { get; private set; }

        #endregion

        #region Lat/Lng/.../GeoWidth/GeoHeight/Altitude

        /// <summary>
        /// The latitude of this shape.
        /// </summary>
        public Latitude  Latitude   { get; private set; }

        /// <summary>
        /// The longitude of this shape.
        /// </summary>
        public Longitude Longitude  { get; private set; }

        /// <summary>
        /// The altitude of this shape.
        /// </summary>
        public Altitude  Altitude   { get; private set; }


        /// <summary>
        /// The latitude2 of this shape.
        /// </summary>
        public Latitude  Latitude2  { get; private set; }

        /// <summary>
        /// The longitude2 of this shape.
        /// </summary>
        public Longitude Longitude2 { get; private set; }

        /// <summary>
        /// The altitude2 of this shape.
        /// </summary>
        public Altitude  Altitude2  { get; private set; }


        /// <summary>
        /// The geographical width of this shape.
        /// </summary>
        public Latitude  GeoWidth   { get; private set; }

        /// <summary>
        /// The geographical height of this shape.
        /// </summary>
        public Longitude GeoHeight  { get; private set; }

        #endregion

        #region OnScreen...

        public ScreenXY OnScreenUpperLeft   { get; private set; }
        public ScreenXY OnScreenLowerRight  { get; private set; }

        public UInt64   OnScreenWidth       { get; private set; }
        public UInt64   OnScreenHeight      { get; private set; }

        #endregion

        #region ZoomLevel

        private UInt32 _ZoomLevel;

        public UInt32 ZoomLevel
        {

            get
            {
                return _ZoomLevel;
            }

            set
            {

                this._ZoomLevel = value;

                SetScreenGeometry();

            }

        }

        #endregion

        #region Bounds

        public Rect Bounds { get; set; }

        #endregion

        #region Fill-/StrokeColor

        public Color FillColor   { get; set; }

        public Color StrokeColor { get; set; }

        #endregion

        #region DefiningGeometry

        private Geometry _DefiningGeometry;

        protected override Geometry DefiningGeometry
        {
            get
            {

                if (_DefiningGeometry == null)
                    SetScreenGeometry();

                return _DefiningGeometry;

            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region GeoShape(Id, Latitude, Longitude, Altitude, GeoWidth, GeoHeight)

        /// <summary>
        /// Create a new polygon geo shape.
        /// </summary>
        /// <param name="Id">The Id of the shape.</param>
        /// <param name="Latitude">The latitude of the shape center.</param>
        /// <param name="Longitude">The longitude of the shape center.</param>
        /// <param name="Altitude">The altitude of the shape center.</param>
        /// <param name="GeoWidth">The geographical width of the shape center.</param>
        /// <param name="GeoHeight">The geographical height of the shape center.</param>
        public GeoShape(String[]   Geometries,
                        Latitude   Latitude,
                        Longitude  Longitude,
                        Altitude   Altitude,
                        Latitude   Latitude2,
                        Longitude  Longitude2,
                        Color      FillColor,
                        Color      StrokeColor,
                        Double     StrokeThickness)

        {

            this.Id         = Id;
            this.Latitude   = Latitude;
            this.Longitude  = Longitude;
            this.Altitude   = Altitude;
            this.Latitude2  = Latitude2;
            this.Longitude2 = Longitude2;

            this.GeoWidth   = new Latitude (Latitude2. Value - Latitude. Value);
            this.GeoHeight  = new Longitude(Longitude2.Value - Longitude.Value);

            var DrawingGroup = new DrawingGroup();
                DrawingGroup.Children.Add(new GeometryDrawing(new SolidColorBrush(FillColor), new Pen(new SolidColorBrush(StrokeColor), StrokeThickness), PathGeometry.Parse(Geometries[8])));

            this.Fill = new DrawingBrush() {
                Drawing   = DrawingGroup,
                //Viewport = new Rect(0, 0, 1, 1),
                TileMode  = TileMode.None,
                Stretch   = Stretch.UniformToFill
            };

            Bounds = DrawingGroup.Bounds;

        }

        #endregion

        #region GeoShape(Id, GeoCoordinates, FillColor, StrokeColor, StrokeThickness)

        /// <summary>
        /// Create a new polygon geo shape.
        /// </summary>
        /// <param name="Id">The Id of the shape.</param>
        public GeoShape(String                      Id,
                        IEnumerable<GeoCoordinate>  GeoCoordinates,
                        Color                       FillColor,
                        Color                       StrokeColor,
                        Double                      StrokeThickness)
        {

            this.Id               = Id;
            this.GeoCoordinates   = GeoCoordinates;

            #region Get Min/Max-Values

            var MinLat = Double.MaxValue;
            var MaxLat = Double.MinValue;

            var MinLng = Double.MaxValue;
            var MaxLng = Double.MinValue;

            var MinAlt = Double.MaxValue;
            var MaxAlt = Double.MinValue;

            foreach (var GeoCoordinate in GeoCoordinates)
            {

                if (GeoCoordinate.Latitude.Value  < MinLat)
                    MinLat = GeoCoordinate.Latitude.Value;

                if (GeoCoordinate.Longitude.Value < MinLng)
                    MinLng = GeoCoordinate.Longitude.Value;

                if (GeoCoordinate.Altitude.Value  < MinAlt)
                    MinAlt = GeoCoordinate.Altitude.Value;


                if (GeoCoordinate.Latitude.Value  > MaxLat)
                    MaxLat = GeoCoordinate.Latitude.Value;

                if (GeoCoordinate.Longitude.Value > MaxLng)
                    MaxLng = GeoCoordinate.Longitude.Value;

                if (GeoCoordinate.Altitude.Value  > MaxAlt)
                    MaxAlt = GeoCoordinate.Altitude.Value;

            }

            this.Latitude   = new Latitude (MinLat);
            this.Longitude  = new Longitude(MinLng);
            this.Altitude   = new Altitude (MinAlt);
            this.Latitude2  = new Latitude (MaxLat);
            this.Longitude2 = new Longitude(MaxLng);
            this.Altitude2  = new Altitude (MaxAlt);

            #endregion

            this.GeoWidth         = new Latitude (Latitude2. Value - Latitude. Value);
            this.GeoHeight        = new Longitude(Longitude2.Value - Longitude.Value);

            this.FillColor        = FillColor;
            this.StrokeColor      = StrokeColor;
            this.StrokeThickness  = StrokeThickness;

        }

        #endregion

        #endregion


        #region (private) SetScreenGeometry()

        private void SetScreenGeometry()
        {

            this.OnScreenUpperLeft  = GeoCalculations.GeoCoordinate2ScreenXY(Latitude,  Longitude,  _ZoomLevel);
            this.OnScreenLowerRight = GeoCalculations.GeoCoordinate2ScreenXY(Latitude2, Longitude2, _ZoomLevel);

            this.OnScreenWidth      = (UInt64) Math.Abs(OnScreenLowerRight.X - OnScreenUpperLeft.X);
            this.OnScreenHeight     = (UInt64) Math.Abs(OnScreenLowerRight.Y - OnScreenUpperLeft.Y);

            if (GeoCoordinates != null)
            {

                var DrawingGroup = new DrawingGroup();

                DrawingGroup.Children.Add(
                    new GeometryDrawing(
                        new SolidColorBrush(FillColor),
                        new Pen(new SolidColorBrush(StrokeColor),
                                StrokeThickness),
                        PathGeometry.Parse(GeoCoordinates.GeoCoordinates2ShapeDefinition(OnScreenUpperLeft, _ZoomLevel, true).Value)
                    )
                );

                this.Fill = new DrawingBrush()
                {
                    Drawing   = DrawingGroup,
                    //Viewport = new Rect(0, 0, 1, 1),
                    TileMode  = TileMode.None,
                    Stretch   = Stretch.UniformToFill
                };

                Bounds = DrawingGroup.Bounds;

            }

            this._DefiningGeometry = new RectangleGeometry() {
                Rect = new Rect(new Size(OnScreenWidth, Bounds.Height / Bounds.Width * OnScreenWidth))
            };

        }

        #endregion


        #region IComparable<Identifier/GeoShape> Members

        #region CompareTo(Identifier)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Identifier">An object to compare with.</param>
        public Int32 CompareTo(String Identifier)
        {

            if ((Object) Identifier == null)
                throw new ArgumentNullException("The given feature identifier must not be null!");

            return this.Id.CompareTo(Identifier);

        }

        #endregion

        #region CompareTo(GeoShape)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Identifier">An object to compare with.</param>
        public Int32 CompareTo(GeoShape GeoShape)
        {

            if ((Object) GeoShape == null)
                throw new ArgumentNullException("The given feature must not be null!");

            return this.Id.CompareTo(GeoShape.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Identifier/GeoShape> Members

        #region Equals(Identifier)

        /// <summary>
        /// Compares two feature identifiers for equality.
        /// </summary>
        /// <param name="Identifier">An identifier to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(String Identifier)
        {

            if ((Object) Identifier == null || Identifier == "")
                return false;

            return this.Id.Equals(Identifier);

        }

        #endregion

        #region Equals(GeoShape)

        /// <summary>
        /// Compares two features for equality.
        /// </summary>
        /// <param name="FeatureLayer">A feature to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(GeoShape GeoShape)
        {

            if ((Object) GeoShape == null)
                return false;

            return this.Id.Equals(GeoShape.Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Returns the hash code for this feature layer.
        /// </summary>
        public new Int32 GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion

    }

}

