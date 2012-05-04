/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <achim@graph-database.org>
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
using System.Collections.Generic;
using System.Collections.Concurrent;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// A geographical coordinate or position on a map.
    /// </summary>
    public class GeoCoordinate : IEquatable<GeoCoordinate>,
                                 IComparable<GeoCoordinate>,
                                 IComparable
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

        #region Properties

        /// <summary>
        /// The Latitude.
        /// </summary>
        public Double Latitude  { get; private set; }

        /// <summary>
        /// The Longitude.
        /// </summary>
        public Double Longitude { get; private set; }

        #endregion

        #region Constructor(s)

        #region GeoCoordinate(Latitude, Longitude)

        /// <summary>
        /// Create a new geographical coordinate or position on a map.
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="Longitude"></param>
        public GeoCoordinate(Double Latitude, Double Longitude)
        {
            this.Latitude  = Latitude;
            this.Longitude = Longitude;
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

            GeoCoordinate GeoPosition;

            if (GeoCoordinate.TryParseString(GeoString, out GeoPosition))
                return GeoPosition;

            return null;

        }

        #endregion

        #region ParseString(GeoString, out GeoPosition)

        /// <summary>
        /// Attempts to parse the given string as a geo position/coordinate.
        /// </summary>
        /// <param name="GeoString">A string to parse.</param>
        /// <returns>True if success, false otherwise</returns>
        public static Boolean TryParseString(String GeoString, out GeoCoordinate GeoPosition)
        {

            var Match = IsDecimalGeoPositionRegExpr.Match(GeoString);

            if (Match.Success)
            {

                var Latitude = Double.Parse(Match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture);

                if (Match.Groups[2].Value == "S")
                    Latitude = -1 * Latitude;

                var Longitude = Double.Parse(Match.Groups[3].Value, NumberStyles.Float, CultureInfo.InvariantCulture);

                if (Match.Groups[4].Value == "W")
                    Longitude = -1 * Longitude;

                GeoPosition = new GeoCoordinate(Latitude, Longitude);
                return true;

            }

            Match = IsSignedDecimalGeoPositionRegExpr.Match(GeoString);

            if (Match.Success)
            {

                var Latitude  = Double.Parse(Match.Groups[1].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);
                var Longitude = Double.Parse(Match.Groups[2].Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

                GeoPosition = new GeoCoordinate(Latitude, Longitude);
                return true;

            }

            GeoPosition = null;
            return false;

        }

        #endregion


        public bool Equals(GeoCoordinate other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(GeoCoordinate other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }


        #region GetHashCode()

        /// <summary>
        /// Return the hashcode of this object.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return (-1*Latitude).GetHashCode() ^ Longitude.GetHashCode();
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
                                         Math.Abs(Latitude).ToString().Replace(',', '.'),
                                         (this.Latitude  < 0) ? "S" : "N",
                                         Math.Abs(Longitude).ToString().Replace(',', '.'),
                                         (this.Longitude < 0) ? "W" : "E");


                // 49° 26' 56.5'' N, 11° 4' 29.6'' E
                case GeoFormat.Sexagesimal:

                    var Latitude_Grad       = (this.Latitude > 0) ? (UInt32) Math.Abs(Math.Floor(this.Latitude)) : (UInt32) Math.Abs(Math.Floor(this.Latitude) + 1);
                    var Latitude_Minute_dec = (Math.Abs(this.Latitude) - Latitude_Grad) * 60;
                    var Latitude_Minute     = (UInt32) Math.Floor(Latitude_Minute_dec);
                    var Latitude_Second_dec = (Latitude_Minute_dec - Latitude_Minute) * 60;

                    var Longitude_Grad       = (this.Longitude > 0) ? (UInt32) Math.Abs(Math.Floor(this.Longitude)) : (UInt32) Math.Abs(Math.Floor(this.Longitude) + 1);
                    var Longitude_Minute_dec = (Math.Abs(this.Longitude) - Longitude_Grad) * 60;
                    var Longitude_Minute     = (UInt32) Math.Floor(Longitude_Minute_dec);
                    var Longitude_Second_dec = (Longitude_Minute_dec - Longitude_Minute) * 60;

                    return String.Format("{0}° {1}' {2}'' {3}, {4}° {5}' {6}'' {7}",
                                         Latitude_Grad,  Latitude_Minute,  Latitude_Second_dec,  (this.Latitude  < 0) ? "S" : "N",
                                         Longitude_Grad, Longitude_Minute, Longitude_Second_dec, (this.Longitude < 0) ? "W" : "E");

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
            return String.Concat("Latitude = ", Latitude, ", Longitude = ", Longitude);
        }

        #endregion

    }

}
