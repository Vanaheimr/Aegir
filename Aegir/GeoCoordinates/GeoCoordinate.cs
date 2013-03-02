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
using System.Text.RegularExpressions;
using System.Globalization;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{


    /// <summary>
    /// Convert to Radians.
    /// </summary>
    /// <param name="val">The value to convert to radians</param>
    /// <returns>The value in radians</returns>
    public static class NumericExtensions
    {

        public static Double ToRadians(this Double Value)
        {
            return Value * (Math.PI / 180);
        }

        public static Double ToRadians(this Latitude Latitude)
        {
            return Latitude.Value * (Math.PI / 180);
        }

        public static Double ToRadians(this Longitude Longitude)
        {
            return Longitude.Value * (Math.PI / 180);
        }


        public static Double ToDegree(this Double Value)
        {
            return Value * (180 / Math.PI);
        }

        public static Double ToDegree(this Latitude Latitude)
        {
            return Latitude.Value * (180 / Math.PI);
        }

        public static Double ToDegree(this Longitude Longitude)
        {
            return Longitude.Value * (180 / Math.PI);
        }

    }



    /// <summary>
    /// A geographical coordinate or position on a map.
    /// </summary>
    public class GeoCoordinate : IGeoCoordinate,
                                 IEquatable<GeoCoordinate>,
                                 IComparable<GeoCoordinate>

    {

        #region (static) Regular expressions

        /// <summary>
        /// The regular expression init string for matching decimal numbers.
        /// </summary>
        public static String IsDecimal_RegExprString                  = "([0-9]+[\\.\\,]?[0-9]*)";

        /// <summary>
        /// The regular expression init string for matching signed decimal numbers.
        /// </summary>
        public static String IsSignedDecimal_RegExprString            = "([-]?[0-9]+[\\.\\,]?[0-9]*)";

        /// <summary>
        /// The regular expression init string for matching comma seperators.
        /// </summary>
        public static String MayBeSeperator_RegExprString             = "[\\s,;]+";

        /// <summary>
        /// The regular expression init string for matching decimal geo positions/coordinates.
        /// </summary>
        public static String IsDecimalGeoPosition_RegExprString       = IsDecimal_RegExprString + "[°]?[\\s]+([SN]?)" +
                                                                        MayBeSeperator_RegExprString +
                                                                        IsDecimal_RegExprString + "[°]?[\\s]+([EWO]?)";

        /// <summary>
        /// The regular expression init string for matching signed decimal geo positions/coordinates.
        /// </summary>
        public static String IsSignedDecimalGeoPosition_RegExprString = IsSignedDecimal_RegExprString + "[°]?" +
                                                                        MayBeSeperator_RegExprString +
                                                                        IsSignedDecimal_RegExprString + "[°]?";

        /// <summary>
        /// The regular expression init string for matching sexagesimal geo positions/coordinates.
        /// </summary>
        public static String IsSexagesimalGeoPosition_RegExprString   = "([-]?[0-9])+°[\\s]+([0-9])+'[\\s]+([0-9]+[\\.\\,]?[0-9]*)''[\\s]+([SN]?)" +
                                                                        MayBeSeperator_RegExprString +
                                                                        "([-]?[0-9])+°[\\s]+([0-9])+'[\\s]+([0-9]+[\\.\\,]?[0-9]*)''[\\s]+([EWO]?)";

        /// <summary>
        /// A regular expression for matching decimal geo positions/coordinates.
        /// </summary>
        public static Regex  IsDecimalRegExpr                         = new Regex(IsDecimal_RegExprString);

        /// <summary>
        /// A regular expression for matching decimal geo positions/coordinates.
        /// </summary>
        public static Regex  IsDecimalGeoPositionRegExpr              = new Regex(IsDecimalGeoPosition_RegExprString);

        /// <summary>
        /// A regular expression for matching signed decimal geo positions/coordinates.
        /// </summary>
        public static Regex  IsSignedDecimalGeoPositionRegExpr        = new Regex(IsSignedDecimalGeoPosition_RegExprString);

        /// <summary>
        /// A regular expression for matching sexagesimal geo positions/coordinates.
        /// </summary>
        public static Regex  IsSexagesimalGeoPositionRegExpr          = new Regex(IsSexagesimalGeoPosition_RegExprString);

        #endregion

        #region Data

        /// <summary>
        /// Min latitude: -90 degree
        /// </summary>
        public static Latitude  MinLatitude  =  new Latitude(-90);

        /// <summary>
        /// Max latitude: +90 degree 
        /// </summary>
        public static Latitude  MaxLatitude  = new Latitude(90);

        /// <summary>
        /// Min longitude: -180 degree
        /// </summary>
        public static Longitude MinLongitude = new Longitude(-180);

        /// <summary>
        /// Max longitude: +180 degree
        /// </summary>
        public static Longitude MaxLongitude = new Longitude(180);

        #endregion

        #region Properties

        #region Latitude

        /// <summary>
        /// The Latitude (south to nord).
        /// </summary>
        public Latitude Latitude { get; private set; }

        #endregion

        #region Longitude

        /// <summary>
        /// The Longitude (parallel to equator).
        /// </summary>
        public Longitude Longitude { get; private set; }

        #endregion

        #endregion

        #region Constructor(s)

        #region GeoCoordinate(Latitude, Longitude)

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="Latitude">The Latitude (south to nord).</param>
        /// <param name="Longitude">The Longitude (parallel to equator).</param>
        public GeoCoordinate(Latitude Latitude, Longitude Longitude)
        {

            #region Initial checks

            if (Latitude  < MinLatitude)
                throw new ArgumentException("The latitude value must be at least "  + MinLatitude + "°!");

            if (Latitude  > MaxLatitude)
                throw new ArgumentException("The latitude value must be at most "   + MaxLatitude + "°!");

            if (Longitude < MinLongitude)
                throw new ArgumentException("The longitude value must be at least " + MinLongitude + "°!");

            if (Longitude > MaxLongitude)
                throw new ArgumentException("The longitude value must be at most "  + MaxLongitude + "°!");

            #endregion

            this.Latitude  = Latitude;
            this.Longitude = Longitude;

        }

        #endregion

        #region GeoCoordinate(GeoString)

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="GeoString">A string representation of a geo coordinate.</param>
        public GeoCoordinate(String GeoString)
        {

            #region Initial checks

            if (GeoString == null || GeoString == "")
                throw new ArgumentNullException("The string represenation of the geo coordinate must not be null or empty!");

            #endregion

            if (!TryParseString(GeoString, (lat, lng) => { this.Latitude = lat; this.Longitude = lng; }))
                throw new ArgumentException("The string represenation of the geo coordinate is invalid!");

        }

        #endregion

        #endregion


        #region ParseString(GeoString)

        /// <summary>
        /// Parses the given string as a geo position/coordinate.
        /// </summary>
        /// <param name="GeoString">A string to parse.</param>
        /// <returns>A new geo position or null.</returns>
        public static GeoCoordinate ParseString(String GeoString)
        {

            GeoCoordinate GeoCoordinate;

            if (GeoCoordinate.TryParseString(GeoString, out GeoCoordinate))
                return GeoCoordinate;

            return null;

        }

        #endregion

        #region ParseString(GeoString, Processor)

        /// <summary>
        /// Parses the given string as a geo position/coordinate.
        /// </summary>
        /// <param name="GeoString">A string to parse.</param>
        /// <returns>A new geo position or null.</returns>
        public static T ParseString<T>(String GeoString, Func<Latitude, Longitude, T> Processor)
        {

            T _T = default(T);

            if (TryParseString<T>(GeoString, Processor, out _T))
                return _T;

            return default(T);

        }

        #endregion

        #region TryParseString(GeoString, out GeoCoordinate)

        /// <summary>
        /// Attempts to parse the given string as a geo position/coordinate.
        /// </summary>
        /// <param name="GeoString">A string to parse.</param>
        /// <param name="GeoCoordinate">The parsed geo coordinate.</param>
        /// <returns>True if success, false otherwise</returns>
        public static Boolean TryParseString(String GeoString, out GeoCoordinate GeoCoordinate)
        {

            return TryParseString<GeoCoordinate>(GeoString, (lat, lng) => new GeoCoordinate(lat, lng), out GeoCoordinate);

        }

        #endregion

        #region TryParseString(GeoString, Processor)

        /// <summary>
        /// Attempts to parse the given string as a geo position/coordinate.
        /// </summary>
        /// <param name="GeoString">A string to parse.</param>
        /// <param name="Processor">A delegate to process the parsed latitude and longitude.</param>
        /// <returns>True if success, false otherwise</returns>
        public static Boolean TryParseString(String GeoString, Action<Latitude, Longitude> Processor)
        {

            Boolean _Boolean;

            return TryParseString<Boolean>(GeoString, (lat, lng) => { Processor(lat, lng); return true; }, out _Boolean);

        }

        #endregion

        #region TryParseString(GeoString, Processor, out Value)

        /// <summary>
        /// Attempts to parse the given string as a geo position/coordinate.
        /// </summary>
        /// <param name="GeoString">A string to parse.</param>
        /// <param name="Processor">A delegate to process the parsed latitude and longitude.</param>
        /// <param name="Value">The processed value.</param>
        /// <returns>True if success, false otherwise</returns>
        public static Boolean TryParseString<T>(String GeoString, Func<Latitude, Longitude, T> Processor, out T Value)
        {

            var Match = IsDecimalGeoPositionRegExpr.Match(GeoString);

            if (Match.Success)
            {

                var Latitude = Double.Parse(Match.Groups[1].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

                if (Match.Groups[2].Value == "S")
                    Latitude = -1 * Latitude;

                var Longitude = Double.Parse(Match.Groups[3].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

                if (Match.Groups[4].Value == "W")
                    Longitude = -1 * Longitude;

                Value = Processor(new Latitude(Latitude), new Longitude(Longitude));
                return true;

            }

            Match = IsSignedDecimalGeoPositionRegExpr.Match(GeoString);

            if (Match.Success)
            {

                var Latitude  = Double.Parse(Match.Groups[1].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);
                var Longitude = Double.Parse(Match.Groups[2].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

                Value = Processor(new Latitude(Latitude), new Longitude(Longitude));
                return true;

            }

            Value = default(T);
            return false;

        }

        #endregion


        #region DistanceTo(Target, EarthRadiusInKM = 6371)

        /// <summary>
        /// Calculate the distance between two geo coordinates in kilometers.
        /// </summary>
        /// <see cref="http://www.movable-type.co.uk/scripts/latlong.html"/>
        /// <seealso cref="http://en.wikipedia.org/wiki/Haversine_formula"/>
        /// <param name="Target">Another geo coordinate</param>
        /// <param name="EarthRadiusInKM">The currently accepted (WGS84) earth radius at the equator is 6378.137 km and 6356.752 km at the polar caps. For aviation purposes the FAI uses a radius of 6371.0 km.</param>
        public Double DistanceTo(GeoCoordinate Target, UInt32 EarthRadiusInKM = 6371)
        {

            var dLat = (Target.Latitude.Value  - this.Latitude.Value).ToRadians();
            var dLon = (Target.Longitude.Value - this.Longitude.Value).ToRadians();

            var a = Math.Sin(dLat / 2)                         * Math.Sin(dLat / 2) +
                    Math.Cos(this.Latitude.Value.ToRadians()) * Math.Cos(Target.Latitude.Value.ToRadians()) *
                    Math.Sin(dLon / 2)                         * Math.Sin(dLon / 2);

            // A (surprisingly marginal) performance improvement can be obtained,
            // of course, by factoring out the terms which get squared.
            //return EarthRadiusInKM * 2 * Math.Asin(Math.Sqrt(a));

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusInKM * c;

        }

        #endregion

        #region MidPoint(Target)

        /// <summary>
        /// Returns the midpoint between this point and the supplied point.
        /// </summary>
        /// <see cref="http://www.movable-type.co.uk/scripts/latlong.html"/>
        /// <seealso cref="http://mathforum.org/library/drmath/view/51822.html"/>
        /// <param name="Target">Anothre geo coordinate.</param>
        public GeoCoordinate MidPoint(GeoCoordinate Target)
        {

            var dLat = (Target.Latitude.Value  - this.Latitude.Value).ToRadians();
            var dLon = (Target.Longitude.Value - this.Longitude.Value).ToRadians();

            var Bx = Math.Cos(Target.Latitude.Value.ToRadians()) * Math.Cos(dLon);
            var By = Math.Cos(Target.Latitude.Value.ToRadians()) * Math.Sin(dLon);

            var lat3 = Math.Atan2(Math.Sin(this.Latitude.Value.ToRadians()) +
                                  Math.Sin(Target.Latitude.Value.ToRadians()),
                                  Math.Sqrt((Math.Cos(this.Latitude.Value.ToRadians()) + Bx) *
                                            (Math.Cos(this.Latitude.Value.ToRadians()) + Bx) + By * By));

            var lon3 = this.Longitude.Value.ToRadians() +
                       Math.Atan2(By, Math.Cos(this.Latitude.Value.ToRadians()) + Bx);

                // Normalise to -180 ... +180º
                lon3 = (lon3 + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

            return new GeoCoordinate(
                           new Latitude (lat3.ToDegree()),
                           new Longitude(lon3.ToDegree())
                       );

        }

        #endregion


        #region Operator overloading

        #region Operator == (GeoCoordinate1, GeoCoordinate2)

        /// <summary>
        /// Compares two geo coordinates for equality.
        /// </summary>
        /// <param name="GeoCoordinate1">A geo coordinate.</param>
        /// <param name="GeoCoordinate2">Another geo coordinate.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (GeoCoordinate GeoCoordinate1, GeoCoordinate GeoCoordinate2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(GeoCoordinate1, GeoCoordinate2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GeoCoordinate1 == null) || ((Object) GeoCoordinate2 == null))
                return false;

            return GeoCoordinate1.Equals(GeoCoordinate2);

        }

        #endregion

        #region Operator != (GeoCoordinate1, GeoCoordinate2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="GeoCoordinate1">A geo coordinate.</param>
        /// <param name="GeoCoordinate2">Another geo coordinate.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (GeoCoordinate GeoCoordinate1, GeoCoordinate GeoCoordinate2)
        {
            return !(GeoCoordinate1 == GeoCoordinate2);
        }

        #endregion

        #endregion

        #region CompareTo(GeoCoordinate)

        /// <summary>
        /// Compares two geo coordinates.
        /// </summary>
        /// <param name="GeoCoordinate">Another geo coordinate.</param>
        public Int32 CompareTo(GeoCoordinate GeoCoordinate)
        {

            var lat = GeoCoordinate.Latitude.Value.CompareTo(this.Latitude.Value);

            if (lat != 0)
                return lat;

            return GeoCoordinate.Longitude.Value.CompareTo(this.Longitude.Value);

        }

        #endregion

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given Object must not be null!");

            // Check if the given object can be casted to a GeoCoordinate
            var GeoCoordinate = Object as GeoCoordinate;

            if ((Object) GeoCoordinate == null)
                throw new ArgumentException("The given object is not a GeoCoordinate!");

            return CompareTo(GeoCoordinate);

        }

        #endregion

        #region Equals(IGeoCoordinate)

        /// <summary>
        /// Compares two geo coordinates for equality.
        /// </summary>
        /// <param name="IGeoCoordinate">Another geo coordinate.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(IGeoCoordinate IGeoCoordinate)
        {

            if (IGeoCoordinate.Latitude.Value != this.Latitude.Value)
                return false;

            if (IGeoCoordinate.Longitude.Value != this.Longitude.Value)
                return false;

            return true;

        }

        #endregion

        #region Equals(GeoCoordinate)

        /// <summary>
        /// Compares two geo coordinates for equality.
        /// </summary>
        /// <param name="GeoCoordinate">Another geo coordinate.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(GeoCoordinate GeoCoordinate)
        {

            if (GeoCoordinate.Latitude.Value != this.Latitude.Value)
                return false;

            if (GeoCoordinate.Longitude.Value != this.Longitude.Value)
                return false;

            return true;

        }

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hashcode of this object.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return Latitude.GetHashCode() << 1 ^ Longitude.GetHashCode();
        }

        #endregion

        #region ToGeoString()

        /// <summary>
        /// Returns a user-friendly string representaion.
        /// </summary>
        public String ToGeoString(GeoFormat GeoType = GeoFormat.Decimal)
        {

            switch (GeoType)
            {

                // 49.44903° N, 11.07488° E
                case GeoFormat.Decimal:

                    return String.Format("{0}° {1}, {2}° {3}",
                                         Math.Abs(Latitude.Value).ToString().Replace(',', '.'),
                                         (Latitude.Value  < 0) ? "S" : "N",
                                         Math.Abs(Longitude.Value).ToString().Replace(',', '.'),
                                         (Longitude.Value < 0) ? "W" : "E");


                // 49° 26' 56.5'' N, 11° 4' 29.6'' E
                case GeoFormat.Sexagesimal:

                    var Latitude_Grad       = (Latitude.Value > 0) ? (UInt32) Math.Abs(Math.Floor(Latitude.Value)) : (UInt32) Math.Abs(Math.Floor(Latitude.Value) + 1);
                    var Latitude_Minute_dec = (Math.Abs(Latitude.Value) - Latitude_Grad) * 60;
                    var Latitude_Minute     = (UInt32) Math.Floor(Latitude_Minute_dec);
                    var Latitude_Second_dec = (Latitude_Minute_dec - Latitude_Minute) * 60;

                    var Longitude_Grad       = (Longitude.Value > 0) ? (UInt32) Math.Abs(Math.Floor(Longitude.Value)) : (UInt32) Math.Abs(Math.Floor(Longitude.Value) + 1);
                    var Longitude_Minute_dec = (Math.Abs(Longitude.Value) - Longitude_Grad) * 60;
                    var Longitude_Minute     = (UInt32) Math.Floor(Longitude_Minute_dec);
                    var Longitude_Second_dec = (Longitude_Minute_dec - Longitude_Minute) * 60;

                    return String.Format("{0}° {1}' {2}'' {3}, {4}° {5}' {6}'' {7}",
                                         Latitude_Grad,  Latitude_Minute,  Latitude_Second_dec,  (Latitude.Value  < 0) ? "S" : "N",
                                         Longitude_Grad, Longitude_Minute, Longitude_Second_dec, (Longitude.Value < 0) ? "W" : "E");

            }

            return String.Empty;

        }

        #endregion

        #region ToString()

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        public override String ToString()
        {
            return String.Concat("Latitude = ", Latitude.Value, ", Longitude = ", Longitude.Value);
        }

        #endregion

    }

}
