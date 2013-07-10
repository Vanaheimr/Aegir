/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

using eu.Vanaheimr.Aegir;
using eu.Vanaheimr.Illias.Commons;
using System.Text;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// A polygon shape on an Aegir map.
    /// </summary>
    public class GeoShape : Shape
    {

        #region Data

        private IEnumerable<GeoCoordinate> _GeoCoordinates;

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
        /// The latitude2 of this shape.
        /// </summary>
        public Latitude  Latitude2  { get; private set; }

        /// <summary>
        /// The longitude2 of this shape.
        /// </summary>
        public Longitude Longitude2 { get; private set; }

        /// <summary>
        /// The geographical width of this shape.
        /// </summary>
        public Latitude  GeoWidth   { get; private set; }

        /// <summary>
        /// The geographical height of this shape.
        /// </summary>
        public Longitude GeoHeight  { get; private set; }

        /// <summary>
        /// The altitude of this shape.
        /// </summary>
        public Altitude  Altitude   { get; private set; }

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
        public GeoShape(String[] Geometries, Latitude Latitude, Longitude Longitude, Altitude Altitude, Latitude Latitude2, Longitude Longitude2, Color StrokeColor, Double StrokeThickness, Color FillColor)
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

        #region GeoShape(Id, GeoCoordinates, StrokeColor, StrokeThickness, FillColor)

        /// <summary>
        /// Create a new polygon geo shape.
        /// </summary>
        /// <param name="Id">The Id of the shape.</param>
        public GeoShape(String Id, IEnumerable<GeoCoordinate> GeoCoordinates, Color StrokeColor, Double StrokeThickness, Color FillColor)
        {

            this.FillColor        = FillColor;
            this.StrokeColor      = StrokeColor;
            this.StrokeThickness  = StrokeThickness;

            this.Id               = Id;
            this.ToolTip          = Id;
            this._GeoCoordinates  = GeoCoordinates;

            var MinLat = Double.MaxValue;
            var MaxLat = Double.MinValue;

            var MinLng = Double.MaxValue;
            var MaxLng = Double.MinValue;

            foreach (var GeoCoordinate in GeoCoordinates)
            {

                if (GeoCoordinate.Latitude.Value < MinLat)
                    MinLat = GeoCoordinate.Latitude.Value;

                if (GeoCoordinate.Latitude.Value > MaxLat)
                    MaxLat = GeoCoordinate.Latitude.Value;


                if (GeoCoordinate.Longitude.Value < MinLng)
                    MinLng = GeoCoordinate.Longitude.Value;

                if (GeoCoordinate.Longitude.Value > MaxLng)
                    MaxLng = GeoCoordinate.Longitude.Value;

            }

            this.Latitude   = new Latitude  (MinLat);
            this.Longitude  = new Longitude (MinLng);
            this.Altitude   = new Altitude  (0);
            this.Latitude2  = new Latitude  (MaxLat);
            this.Longitude2 = new Longitude (MaxLng);

            this.GeoWidth   = new Latitude (Latitude2. Value - Latitude. Value);
            this.GeoHeight  = new Longitude(Longitude2.Value - Longitude.Value);

        }

        #endregion

        #endregion


        #region (private) SetScreenGeometry()

        private void SetScreenGeometry()
        {

            this.OnScreenUpperLeft  = GeoCalculations.WorldCoordinates_2_Screen(Latitude,  Longitude,  _ZoomLevel);
            this.OnScreenLowerRight = GeoCalculations.WorldCoordinates_2_Screen(Latitude2, Longitude2, _ZoomLevel);

            this.OnScreenWidth      = (UInt64) Math.Abs(OnScreenLowerRight.X - OnScreenUpperLeft.X);
            this.OnScreenHeight     = (UInt64) Math.Abs(OnScreenLowerRight.Y - OnScreenUpperLeft.Y);

            if (_GeoCoordinates != null)
            {

                var DrawingGroup = new DrawingGroup();
                DrawingGroup.Children.Add(
                    new GeometryDrawing(
                        new SolidColorBrush(FillColor),
                        new Pen(new SolidColorBrush(StrokeColor),
                                StrokeThickness),
                        PathGeometry.Parse(_GeoCoordinates.GeoCoord2Shape(OnScreenUpperLeft, _ZoomLevel))
                    )
                );

                this.Fill = new DrawingBrush()
                {
                    Drawing = DrawingGroup,
                    //Viewport = new Rect(0, 0, 1, 1),
                    TileMode = TileMode.None,
                    Stretch = Stretch.UniformToFill
                };

                Bounds = DrawingGroup.Bounds;

            }

            this._DefiningGeometry = new RectangleGeometry() {
                Rect = new Rect(new Size(OnScreenWidth, Bounds.Height / Bounds.Width * OnScreenWidth))
            };

        }

        #endregion


        #region IComparable<Identifier> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is a feature.
            var Feature = Object as IFeature;
            if ((Object) Feature == null)
                throw new ArgumentException("The given object is not a map feature!");

            return this.Id.CompareTo(Feature.Id);

        }

        #endregion

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

        #region CompareTo(Feature)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Identifier">An object to compare with.</param>
        public Int32 CompareTo(IFeature Feature)
        {

            if ((Object) Feature == null)
                throw new ArgumentNullException("The given feature must not be null!");

            return this.Id.CompareTo(Feature.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Identifier> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public new Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is a feature.
            var Feature = Object as IFeature;
            if ((Object) Feature == null)
                return false;

            return this.Id.Equals(Feature.Id);

        }

        #endregion

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

        #region Equals(Feature)

        /// <summary>
        /// Compares two features for equality.
        /// </summary>
        /// <param name="FeatureLayer">A feature to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IFeature Feature)
        {

            if ((Object) Feature == null)
                return false;

            return this.Id.Equals(Feature.Id);

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

