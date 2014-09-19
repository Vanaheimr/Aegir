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
using System.Globalization;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// A latitude.
    /// </summary>
    public struct Latitude : IEquatable<Latitude>,
                             IComparable<Latitude>,
                             IComparable

    {

        #region Properties

        private readonly Double _Value;

        /// <summary>
        /// Returns the value of the latitude.
        /// </summary>
        public Double Value
        {
            get
            {
                return _Value;
            }
        }

        #endregion

        #region Constructor(s)

        #region Latitude(Value)

        /// <summary>
        /// Create a new latitude.
        /// </summary>
        /// <param name="Value">The value of the latitude.</param>
        public Latitude(Double Value)
        {
            _Value = Value;
        }

        #endregion

        #endregion


        public static Latitude Parse(String Latitude)
        {
            return new Latitude(Double.Parse(Latitude, CultureInfo.InvariantCulture));
        }

        public static Latitude Parse(String Latitude, IFormatProvider FormatProvider)
        {
            return new Latitude(Double.Parse(Latitude, FormatProvider));
        }

        public static Latitude Parse(String Latitude, NumberStyles NumberStyle)
        {
            return new Latitude(Double.Parse(Latitude, NumberStyle));
        }

        public static Latitude Parse(String Latitude, NumberStyles NumberStyle, IFormatProvider FormatProvider)
        {
            return new Latitude(Double.Parse(Latitude, NumberStyle, FormatProvider));
        }


        #region Distance(OtherLatitude)

        /// <summary>
        /// A method to calculate the distance between two latitudes.
        /// </summary>
        /// <param name="OtherLatitude">Another latitude.</param>
        /// <returns>The distance between a and b.</returns>
        public Double DistanceTo(Latitude OtherLatitude)
        {
            return Math.Abs(_Value - OtherLatitude.Value);
        }

        #endregion


        #region Operator overloading

        #region Operator == (Latitude1, Latitude2)

        /// <summary>
        /// Compares two latitudes for equality.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Latitude Latitude1, Latitude Latitude2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Latitude1, Latitude2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Latitude1 == null) || ((Object) Latitude2 == null))
                return false;

            return Latitude1.Equals(Latitude2);

        }

        #endregion

        #region Operator != (Latitude1, Latitude2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Latitude Latitude1, Latitude Latitude2)
        {
            return !(Latitude1 == Latitude2);
        }

        #endregion

        #region Operator <  (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  < (Latitude Latitude1, Latitude Latitude2)
        {

            if ((Object) Latitude1 == null)
                throw new ArgumentNullException("The given Latitude1 must not be null!");

            if ((Object) Latitude2 == null)
                throw new ArgumentNullException("The given Latitude2 must not be null!");

            return Latitude1.CompareTo(Latitude2) < 0;

        }

        #endregion

        #region Operator <= (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Latitude Latitude1, Latitude Latitude2)
        {
            return !(Latitude1 > Latitude2);
        }

        #endregion

        #region Operator >  (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  > (Latitude Latitude1, Latitude Latitude2)
        {

            if ((Object) Latitude1 == null)
                throw new ArgumentNullException("The given Latitude1 must not be null!");

            if ((Object) Latitude2 == null)
                throw new ArgumentNullException("The given Latitude2 must not be null!");

            return Latitude1.CompareTo(Latitude2) > 0;

        }

        #endregion

        #region Operator >= (Latitude1, Latitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Latitude1">A latitude.</param>
        /// <param name="Latitude2">Another latitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Latitude Latitude1, Latitude Latitude2)
        {
            return !(Latitude1 < Latitude2);
        }

        #endregion

        #endregion

        #region IComparable Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given Object must not be null!");

            return CompareTo((Latitude) Object);

        }

        #endregion

        #region CompareTo(Latitude)

        /// <summary>
        /// Compares two latitudes.
        /// </summary>
        /// <param name="Latitude">Another latitude.</param>
        public Int32 CompareTo(Latitude Latitude)
        {
            return this.Value.CompareTo(Latitude.Value);
        }

        #endregion

        #endregion

        #region IEquatable Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;
            
            try
            {
                return this.Equals((Latitude) Object);
            }
            catch (InvalidCastException)
            {
                return false;
            }

        }

        #endregion

        #region Equals(Latitude)

        /// <summary>
        /// Compares two latitudes for equality.
        /// </summary>
        /// <param name="Latitude">Another latitude.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(Latitude Latitude)
        {
            return this.Value.Equals(Latitude.Value);
        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hashcode of this object.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        public override String ToString()
        {
            return this.Value.ToString();
        }

        #endregion

        #region ToString(FormatProvider)

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        /// <param name="FormatProvider">An object that supplies culture-specific formatting information.</param>
        public String ToString(IFormatProvider FormatProvider)
        {
            return this.Value.ToString(FormatProvider);
        }

        #endregion

    }

}
