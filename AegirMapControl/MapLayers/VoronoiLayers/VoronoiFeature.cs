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
using System.Windows.Shapes;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature on a feature layer of an Aegir map.
    /// </summary>
    public class VoronoiFeature : Shape,
                                  IEquatable<VoronoiFeature>, IComparable<VoronoiFeature>,
                                  IEquatable<String>,         IComparable<String>
    {

        #region Properties

        #region Id

        /// <summary>
        /// The unique identification of this map feature.
        /// </summary>
        public String Id { get; private set; }

        #endregion

        #region Latitude/Longitude/Altitude

        public GeoCoordinate GeoCoordinate { get; private set; }

        /// <summary>
        /// The latitude of the feature.
        /// </summary>
        public Latitude  Latitude   { get { return this.GeoCoordinate.Latitude; } }

        /// <summary>
        /// The longitude of the feature.
        /// </summary>
        public Longitude Longitude  { get { return this.GeoCoordinate.Longitude; } }

        /// <summary>
        /// The altitude to the feature.
        /// </summary>
        public Altitude  Altitude   { get { return this.GeoCoordinate.Altitude; } }

        #endregion

        #region Geometry

        private readonly Geometry Geometry;

        /// <summary>
        /// The geometry of the feature.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                return Geometry;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region Feature(Id, GeoCoordinate)

        /// <summary>
        /// Create a new map feature.
        /// </summary>
        /// <param name="Id">The unique identification of this map feature.</param>
        /// <param name="GeoCoordinate">The geo coordinate of the feature.</param>
        public VoronoiFeature(String         Id,
                              GeoCoordinate  GeoCoordinate)
        {
            this.Id             = Id;
            this.GeoCoordinate  = GeoCoordinate;
        }

        #endregion

        #endregion


        #region IComparable<Identifier/Feature> Members

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
        public Int32 CompareTo(VoronoiFeature Feature)
        {

            if ((Object) Feature == null)
                throw new ArgumentNullException("The given feature must not be null!");

            return this.Id.CompareTo(Feature.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Identifier/Feature> Members

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
        public Boolean Equals(VoronoiFeature Feature)
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

