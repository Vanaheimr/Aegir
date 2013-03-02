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
using System.Windows.Shapes;
using System.Windows.Media;

using de.ahzf.Vanaheimr.Aegir;
using de.ahzf.Illias.Commons;
using System.Windows;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// A shape on an Aegir map.
    /// </summary>
    public abstract class AShape : Shape, IShape
    {

        #region Properties

        /// <summary>
        /// The identification of this shape.
        /// </summary>
        public String    Id         { get; private set; }

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
        /// The geographical width of thsi shape.
        /// </summary>
        public Latitude  Latitude2  { get; private set; }

        /// <summary>
        /// The geographical height of this shape.
        /// </summary>
        public Longitude Longitude2 { get; private set; }



        public UInt32    ZoomLevel  { get; set; }

        public Rect      Bounds     { get; set; }


        #region Geometry

        private readonly Geometry Geometry;

        //protected override Geometry DefiningGeometry
        //{
        //    get
        //    {
        //        return Geometry;
        //    }
        //}

        #endregion

        #region Geometry

        protected override Geometry DefiningGeometry
        {
            get
            {

                var Starts = GeoCalculations.WorldCoordinates_2_Screen(Latitude,  Longitude,  ZoomLevel);
                var Ends   = GeoCalculations.WorldCoordinates_2_Screen(Latitude2, Longitude2, ZoomLevel);
                // Width:  7.788112
                // Height: 9.175257

                var w2 = Ends.Item1 - Starts.Item1;
                var h2 = Ends.Item2 - Starts.Item2;

                return new RectangleGeometry() { Rect = new Rect(new Size(w2, Bounds.Height / Bounds.Width * w2)) }; ;

            }
        }

        #endregion

        #endregion


        #region Constructor(s)

        #region AShape(Id, Latitude, Longitude, Altitude, GeoWidth, GeoHeight)

        /// <summary>
        /// Create a new abstract shape.
        /// </summary>
        /// <param name="Id">The Id of the shape.</param>
        /// <param name="Latitude">The latitude of the shape center.</param>
        /// <param name="Longitude">The longitude of the shape center.</param>
        /// <param name="Altitude">The altitude of the shape center.</param>
        /// <param name="GeoWidth">The geographical width of the shape center.</param>
        /// <param name="GeoHeight">The geographical height of the shape center.</param>
        public AShape(String Id, Latitude Latitude, Longitude Longitude, Altitude Altitude, Latitude Latitude2, Longitude Longitude2)
        {

            this.Id         = Id;
            this.Latitude   = Latitude;
            this.Longitude  = Longitude;
            this.Altitude   = Altitude;
            this.Latitude2  = Latitude2;
            this.Longitude2 = Longitude2;

        }

        #endregion

        #region AShape(Id, Latitude, Longitude, Altitude, GeoWidth, GeoHeight)

        /// <summary>
        /// Create a new abstract shape.
        /// </summary>
        /// <param name="Id">The Id of the shape.</param>
        /// <param name="Latitude">The latitude of the shape center.</param>
        /// <param name="Longitude">The longitude of the shape center.</param>
        /// <param name="Altitude">The altitude of the shape center.</param>
        /// <param name="GeoWidth">The geographical width of the shape center.</param>
        /// <param name="GeoHeight">The geographical height of the shape center.</param>
        public AShape(String[] Geometries, Latitude Latitude, Longitude Longitude, Altitude Altitude, Latitude Latitude2, Longitude Longitude2, Color StrokeColor, Double StrokeThickness, Color FillColor)
        {

            this.Id         = Id;
            this.Latitude   = Latitude;
            this.Longitude  = Longitude;
            this.Altitude   = Altitude;
            this.Latitude2  = Latitude2;
            this.Longitude2 = Longitude2;

            var PathGeometry16 = PathGeometry.Parse(Geometries[8]);

            var GD16 = new GeometryDrawing(new SolidColorBrush(FillColor), new Pen(new SolidColorBrush(StrokeColor), StrokeThickness), PathGeometry16);

            var DrawingGroup = new DrawingGroup();
            DrawingGroup.Children.Add(GD16);

            this.Fill = new DrawingBrush()
            {
                Drawing = DrawingGroup,
                //Viewport = new Rect(0, 0, 1, 1),
                TileMode = TileMode.None,
                Stretch = Stretch.UniformToFill
            };

            Bounds = DrawingGroup.Bounds;
            var w = Bounds.Width;
            var h = Bounds.Height;

        }

        #endregion

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

